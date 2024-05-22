using System.Runtime.InteropServices;

namespace EventPi.Services.Camera;

[StructLayout(LayoutKind.Sequential)]
public struct FrameImageInfo
{
    public Int64 BrightPixels;
    public Int64 DarkPixels;
};