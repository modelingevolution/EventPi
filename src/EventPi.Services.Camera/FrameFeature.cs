using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public unsafe class FrameFeatureAccessor : IDisposable
{
    private readonly MemoryMappedFile _mmf;
    private readonly MemoryMappedViewAccessor _area;
    private readonly FrameImageInfo* _pointer = null;
    private readonly ILogger<FrameFeatureAccessor> _logger;

    public FrameImageInfo FrameFeatures
    {
        get
        {
            if(_pointer != null)
                return *_pointer;
            else
            {
                return new FrameImageInfo();
            }
        }
    }

    public FrameFeatureAccessor(ILogger<FrameFeatureAccessor> logger)
    {
        _logger = logger;

        try
        {
            _mmf = MemoryMappedFile.CreateFromFile("/dev/shm/rpicam_shm", System.IO.FileMode.OpenOrCreate, null, 10000);
            _area = _mmf.CreateViewAccessor(0, 8);
            RuntimeHelpers.PrepareConstrainedRegions();
            byte* pointer = null;
            _area.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
            _pointer = (FrameImageInfo*)pointer;
            _logger.LogInformation("FrameFeatureAccessor initialized");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
      
    }


    public void Dispose()
    {
        _area.SafeMemoryMappedViewHandle.ReleasePointer();
        _area.Dispose();
        _mmf.Dispose();
    }
}