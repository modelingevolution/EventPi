using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using EventPi.Threading;

namespace EventPi.SharedMemory
{
    public class SharedCyclicBuffer : IDisposable
    {
        private readonly long _capacity;
        private readonly MemoryMappedFile _mmf;
        private readonly MemoryMappedViewAccessor _accessor;
        private readonly ISemaphore _availableItemsSem;
        private long _tail = 0;
        private long _head = 0;
        private readonly int SIZE_T;
        private bool _disposed = false;
        public SharedCyclicBuffer(long capacity, int size_t, string shmName)
        {
            SIZE_T = size_t;
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
            _availableItemsSem = SemaphoreFactory.Create($"sem_{Path.GetFileName(shmName)}", 0);
        }


        ~SharedCyclicBuffer()
        {
            Dispose(false);
        }

        public bool Push<T>(T value) where T:struct
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
        public unsafe ref T Pop<T>(T value) where T : struct
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
        public bool Pop<T>(out T value) where T : struct
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
        public SharedCyclicBuffer(long capacity, string shmName)
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
            _availableItemsSem = SemaphoreFactory.Create($"sem_{Path.GetFileName(shmName)}", 0);
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
