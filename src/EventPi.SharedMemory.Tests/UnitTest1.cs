using System.ComponentModel;

namespace EventPi.SharedMemory.Tests
{
    public class SharedCyclicBufferTests
    {
        private bool readOk = false;
        [Fact]
        public void Counter()
        {
            SharedCyclicBuffer write = new SharedCyclicBuffer(120, sizeof(int), "test");
            SharedCyclicBuffer read = new SharedCyclicBuffer(120, sizeof(int), "test");

            Thread reader = new Thread(OnRead);
            reader.Start(read);

            Thread writer = new Thread(OnWrite);
            writer.Start(write);
            
            writer.Join();
            reader.Join();
            Assert.True(readOk);
        }

        private const int ITERATIONS = 2000;
        private unsafe void OnWrite(object? obj)
        {
            SharedCyclicBuffer write = (SharedCyclicBuffer)obj;
            Thread.Sleep(100);
            for (int i = ITERATIONS; i > 0; i--)
            {
                //write.Push(ref i);
                nint ptr =(nint) (&i);
                write.PushBytes(ptr);
                Thread.Sleep(1);
            }
        }
        private unsafe void OnRead(object? obj)
        {
            SharedCyclicBuffer buffer = (SharedCyclicBuffer)obj;
            for (int i = ITERATIONS; i > 0; i--)
            {
                var ptr = (int*)buffer.PopPtr();
                Assert.Equal(i, *ptr);
            }

            readOk = true;
        }
    }
}