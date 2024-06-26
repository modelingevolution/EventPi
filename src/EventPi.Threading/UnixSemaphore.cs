using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EventPi.Threading
{
    public interface ISemaphore : IDisposable
    {
        void Wait();
        void Post();
        void Close();
        void Unlink();
    }

    public static class SemaphoreFactory
    {
        public static ISemaphore Create(string name, int initialCount)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsSemaphore(name, initialCount, int.MaxValue,true);
            return new UnixSemaphore(name, (uint)initialCount, true);
        }
        public static ISemaphore Open(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsSemaphore(name, 0, int.MaxValue, false);
            return new UnixSemaphore(name, 0, false);
        }
    }

    sealed class WindowsSemaphore : ISemaphore
    {
        private readonly Semaphore _semaphore;
        public WindowsSemaphore(string name, int initialCount, int maximumCount, bool create)
        {
            bool createdNew;
            _semaphore = new Semaphore(initialCount, maximumCount, name, out createdNew);
            if (!createdNew & createdNew)
            {
                _semaphore.Dispose();
                throw new InvalidOperationException("Semaphore already created!");
            }
        }
        public void Wait()
        {
            _semaphore.WaitOne();
        }

        public void Close()
        {
            _semaphore.Close();
        }

        public void Unlink()
        {
            _semaphore.Close();
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        
        }
        public void Post()
        {
            _semaphore.Release();
        }
    }
    sealed class UnixSemaphore : ISemaphore
    {
        private static bool _isChecked = false;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Check()
        {
            if(_isChecked) return;
            _isChecked = true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException("This class is only supported on Unix-based systems.");
            string currentDllPath = Assembly.GetExecutingAssembly().Location;
            string currentDllDirectory = Path.GetDirectoryName(currentDllPath) ?? AppContext.BaseDirectory;
            var outputPath = Path.Combine(currentDllDirectory, "sem.so");
            switch (RuntimeInformation.OSArchitecture)
            {
                case Architecture.Arm64:
                    ExtractResourceToFile("sem_arm64.so", outputPath);
                    break;
                case Architecture.X64:
                    ExtractResourceToFile("sem_x64.so", outputPath);
                    break;
                default:
                    throw new PlatformNotSupportedException("This class is only supported on Unix-based systems.");
            }
        }

        static UnixSemaphore() => Check();

        private static void ExtractResourceToFile(string resourceName, string outputPath)
        {
            // Get a reference to the current assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Use the assembly and the resource name to build the full resource name
            string resourceFullName = assembly.GetName().Name + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");

            // Use a stream to get the resource
            using Stream? resourceStream = assembly.GetManifestResourceStream(resourceFullName);
            if (resourceStream == null)
                throw new FileNotFoundException("Could not find the embedded resource", resourceName);

            // Copy the stream content to the output file
            using FileStream outputFileStream = new FileStream(outputPath, FileMode.Create);
            resourceStream.CopyTo(outputFileStream);
        }
        // Import the sem_open function from the libc library
        

        // Oflag and mode constants (for demonstration, adjust as needed)
        private const int O_CREAT = 0x100;
        private const int O_EXCL = 0x80;
        private const uint MODE = 438; // Equivalent to 0666

        private readonly IntPtr _semaphore;
        private readonly string _name;
        private bool _disposed = false;
        public UnixSemaphore(string name, uint initialValue, bool exclusive)
        {
            UnixSemaphore.Check();
            if (!name.StartsWith('/'))
                name = $"/{name}";
            _name = name;
            var flags = exclusive ? (O_EXCL | O_CREAT): O_CREAT;
            _semaphore = Interop.sem_open(name, flags, MODE, initialValue);
            if (_semaphore == (IntPtr)(-1))
            {
                
                //int error = Interop.errno();
                //Console.Error.WriteLine($"Failed to open semaphore. Error code: {-1}");
                throw new InvalidOperationException("Failed to open semaphore.");
            }
            else if (_semaphore == IntPtr.Zero)
            {
                if (exclusive)
                {
                    int c = Interop.sem_unlink(name);
                    //Console.WriteLine($"Unlinking semaphore: {c}");
                    _semaphore = Interop.sem_open(name, flags, MODE, initialValue);
                }

                if (_semaphore == IntPtr.Zero)
                {
                    //int error = Interop.errno();
                    //Console.WriteLine($"Failed to open semaphore. Error code: {0}");
                    throw new InvalidOperationException("Semaphore opened but returned a null pointer.");
                }
                else
                {
                    //Console.WriteLine("Semaphore successfully created/opened.");
                }
            }
        }

        public void Wait()
        {
            if (Interop.sem_wait(_semaphore) != 0)
                throw new InvalidOperationException("Failed to wait on semaphore.");
        }

        public void Post()
        {
            if (Interop.sem_post(_semaphore) != 0)
                throw new InvalidOperationException("Failed to post semaphore.");
        }

        public void Close()
        {
            _disposed = true;
            if (Interop.sem_close(_semaphore) != 0)
                throw new InvalidOperationException("Failed to close semaphore.");
        }

        public void Unlink()
        {
            if (Interop.sem_unlink(_name) != 0)
                throw new InvalidOperationException("Failed to unlink semaphore.");
        }
        ~UnixSemaphore()
        {
            Dispose();
        }
        /// <summary>
        /// Close and Unlink
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            if (_semaphore != IntPtr.Zero)
            {
                Interop.sem_close(_semaphore);
                Interop.sem_unlink(_name);
            }
            GC.SuppressFinalize(this);
        }
    }
}
