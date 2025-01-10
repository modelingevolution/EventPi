using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace EventPi.Abstractions;

public static class Extensions
{
    public static byte[] ToHash(this string t)
    {
        using (SHA256 shA256 = SHA256.Create())
        {
            byte[] hash = shA256.ComputeHash(Encoding.Default.GetBytes(t));
            ulong uint64_1 = BitConverter.ToUInt64(hash, 0);
            ulong uint64_2 = BitConverter.ToUInt64(hash, 8);
            ulong uint64_3 = BitConverter.ToUInt64(hash, 16);
            ulong uint64_4 = BitConverter.ToUInt64(hash, 24);
            ulong num1 = uint64_1 ^ uint64_3;
            ulong num2 = uint64_2 ^ uint64_4;
            Memory<byte> memory = new Memory<byte>(new byte[16]);
            BitConverter.TryWriteBytes(memory.Span, num1);
            BitConverter.TryWriteBytes(memory.Slice(8, 8).Span, num2);
            return memory.ToArray();
        }
    }
    public static Guid ToGuid(this string t) => new Guid(t.ToHash());
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Area(this Rectangle r) => r.Width * r.Height;
    


}