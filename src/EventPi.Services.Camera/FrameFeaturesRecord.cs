namespace EventPi.Services.Camera;

public record FrameFeaturesRecord
{
    public float LargestSharedArea { get; set; }
    public float LargestSharedAreaWidth { get; set; }
    public float LargestSharedAreaHeight { get; set; }
    public float SharedAreasAmount { get; set; }
    public float TotalSharedArea { get; set; }
    public int TotalBrightPixels { get; set; }
    public int TotalDarkPixels { get; set; }
    public float Lux { get; set; }

    public override string ToString()
    {
        return $"Bright pixels: {TotalBrightPixels} \n Dark pixels: {TotalDarkPixels} \n Lux: {Lux}";
    }
}