using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using EventPi.Threading;

namespace EventPi.SharedMemory
{
    public enum OpenMode
    {
        OpenExistingForWriting,
        CreateNewForReading
    }
    public class SharedCyclicBuffer : IDisposable
    {
        private readonly long _capacity;
        private readonly MemoryMappedFile _mmf;
        private readonly MemoryMappedViewAccessor _accessor;
        private readonly string _shmName;
        private readonly ISemaphore _availableItemsSem;
        private long _tail = 0;
        private long _head = 0;
        private readonly int _frameSize;
        private bool _disposed = false;
        public ulong Capacity => (ulong)_capacity;
        public string Name => _shmName;
        public long _totalBufferSize;
        public int FrameSize => _frameSize;
        public long TotalBufferSize => _totalBufferSize;

        private ulong _frameCounter;
        public SharedCyclicBuffer(long capacity, int frameSize, string shmName, OpenMode mode)
        {
            _frameCounter = 0;
            _frameSize = frameSize;
            if (shmName == null)
                throw new ArgumentNullException(nameof(shmName));
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _capacity = capacity;
            _shmName = shmName;
            if (!shmName.StartsWith("/dev/shm") && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _shmName = shmName = $"/dev/shm/{shmName}";
            // Calculate the size needed for the buffer
            _totalBufferSize = _capacity * _frameSize + _capacity*sizeof(ulong);

            // Create or open the memory-mapped file
            Debug.WriteLine($"Opening: {shmName}");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _mmf = MemoryMappedFile.CreateOrOpen(shmName, _totalBufferSize,
                    MemoryMappedFileAccess.ReadWrite,
                    MemoryMappedFileOptions.None,
                    HandleInheritability.Inheritable);
            else
            {
                try
                {
                    _mmf = MemoryMappedFile.CreateFromFile(shmName,
                        FileMode.OpenOrCreate, null,
                        _totalBufferSize);
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    //The capacity may not be smaller than the file size. (Parameter 'capacity')
                    File.Delete(shmName);
                    _mmf = MemoryMappedFile.CreateFromFile(shmName,
                        FileMode.OpenOrCreate, null,
                        _totalBufferSize);
                }
            }

            // Create a view accessor for the memory-mapped file
            _accessor = _mmf.CreateViewAccessor();

            // Open or create a semaphore for synchronization
            string semName = $"sem_{Path.GetFileName(shmName)}";
            _availableItemsSem = mode == OpenMode.CreateNewForReading ? SemaphoreFactory.Create(semName, 0)
                : SemaphoreFactory.Open(semName);
        }


        ~SharedCyclicBuffer()
        {
            Dispose(false);
        }

        public unsafe void Clear()
        {
            byte* dst = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref dst);
            Span<byte> b = new Span<byte>(dst, (int)TotalBufferSize);
            b.Clear();
            _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        }
        public unsafe bool PushBytes(IntPtr value)
        {
            return PushBytes((byte*)value);
        }
        public unsafe bool PushBytes(byte* value)
        {
            long nxTail = (_tail + 1) % _capacity;

            // Calculate the byte offset in the memory-mapped file
            long offset = _tail * (_frameSize + sizeof(ulong));

            byte* dst = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref dst);
            dst += offset;
            
            Buffer.MemoryCopy(value,dst,_frameSize, _frameSize);
            
            var controlPtr = (ulong*)(dst + _frameSize);
            *controlPtr = _frameCounter++;

            _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            _accessor.Flush();
            
            _tail = nxTail;

            // Release the semaphore
            _availableItemsSem.Post();
            return true;
        }
        public bool PushBytes(byte[] value)
        {
            long nxTail = (_tail + 1) % _capacity;

            // Calculate the byte offset in the memory-mapped file
            long offset = _tail * _frameSize;

            _accessor.WriteArray(offset, value, 0, _frameSize);
            _tail = nxTail;

            // Release the semaphore
            _availableItemsSem.Post();
            return true;
        }
        public bool Push<T>(ref T value) where T:struct
        {
            long nxTail = (_tail + 1) % _capacity;

            // Calculate the byte offset in the memory-mapped file
            long offset = _tail * _frameSize;
            _accessor.Write(offset, ref value);
            _tail = nxTail;

            // Release the semaphore
            _availableItemsSem.Post();
            return true;
        }

        public unsafe IntPtr PopPtr()
        {
            _availableItemsSem.Wait();

            long offset = _head * (_frameSize + sizeof(ulong));
            _head = (_head + 1) % _capacity;

            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            
            ptr += offset;

            ulong* controlPtr = (ulong*)(ptr + _frameSize);
            var control = *controlPtr;
            if (_frameCounter == 0) _frameCounter = control;
            if (_frameCounter++ != control)
                throw new InvalidOperationException($"Unordered memory, expecting: {_frameCounter-1} but received: {control}");

            
            return (IntPtr)ptr;
        }

        public void ReleaseFrame()
        {
            _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        }
        public unsafe ref T Pop<T>(T value) where T : struct
        {
            _availableItemsSem.Wait();

            long offset = _head * _frameSize;
            _head = (_head + 1) % _capacity;

            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            ptr += offset;
            T* p = (T*)ptr;
            return ref (*p); // Return a pointer to the item
        }
        public bool Pop<T>(out T value) where T : struct
        {
            _availableItemsSem.Wait();

            // Calculate the byte offset in the memory-mapped file
            long offset = _head * _frameSize;

            _accessor.Read<T>(offset, out value);
            _head = (_head + 1) % _capacity;

            return true;
        }



        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            _accessor?.Dispose();
            _mmf?.Dispose();
            _availableItemsSem?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

   
    public class SharedCyclicBuffer<T>  : IDisposable
        where T : struct
    {
        private readonly long _capacity;
        private readonly MemoryMappedFile _mmf;
        private readonly MemoryMappedViewAccessor _accessor;
        private readonly ISemaphore _availableItemsSem;
        private long _tail = 0;
        private long _head = 0;
        private static readonly int SIZE_T = Marshal.SizeOf(typeof(T));
        private bool _disposed = false;
        public SharedCyclicBuffer(long capacity, string shmName, OpenMode mode)
        {
            if (shmName == null)
                throw new ArgumentNullException(nameof(shmName));
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _capacity = capacity;
            if (!shmName.StartsWith("/dev/shm") && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                shmName = $"/dev/shm/{shmName}";
            // Calculate the size needed for the buffer
            long shmSize = _capacity * SIZE_T;

            // Create or open the memory-mapped file
            Debug.WriteLine($"Opening: {shmName}");

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _mmf = MemoryMappedFile.CreateOrOpen(shmName, shmSize,
                    MemoryMappedFileAccess.Read,
                    MemoryMappedFileOptions.None,
                    HandleInheritability.Inheritable);
            else
                _mmf = MemoryMappedFile.CreateFromFile(shmName, 
                    FileMode.OpenOrCreate, null, 
                    shmSize);

            // Create a view accessor for the memory-mapped file
            _accessor = _mmf.CreateViewAccessor();

            // Open or create a semaphore for synchronization
            _availableItemsSem = mode == OpenMode.CreateNewForReading ? 
                SemaphoreFactory.Create($"sem_{Path.GetFileName(shmName)}", 0) :
                SemaphoreFactory.Open($"sem_{Path.GetFileName(shmName)}");
        }

        
        ~SharedCyclicBuffer()
        {
            Dispose(false);
        }

        public bool Push(T value)
        {
            long nxTail = (_tail + 1) % _capacity;

            // Calculate the byte offset in the memory-mapped file
            long offset = _tail * SIZE_T;
            _accessor.Write(offset, ref value);
            _tail = nxTail;

            // Release the semaphore
            _availableItemsSem.Post();
            return true;
        }

        public unsafe IntPtr PopPtr()
        {
            _availableItemsSem.Wait();

            long offset = _head * SIZE_T;
            _head = (_head + 1) % _capacity;

            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            ptr += offset;
            return (IntPtr)ptr;
        }
        public unsafe ref T Pop()
        {
            _availableItemsSem.Wait();

            long offset = _head * SIZE_T;
            _head = (_head + 1) % _capacity;

            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            ptr += offset;
            T* p = (T*)ptr;
            return ref (*p); // Return a pointer to the item
        }
        public bool Pop(out T value)
        {
            _availableItemsSem.Wait();

            // Calculate the byte offset in the memory-mapped file
            long offset = _head * SIZE_T;

            _accessor.Read<T>(offset, out value);
            _head = (_head + 1) % _capacity;

            return true;
        }

       

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            _accessor.Dispose();
            _mmf.Dispose();
            _availableItemsSem.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
