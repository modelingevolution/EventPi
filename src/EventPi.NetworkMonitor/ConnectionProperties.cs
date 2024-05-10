namespace NetworkManager.DBus;

record ConnectionProperties
{
    internal bool Unsaved { get; set; } = default!;
    internal uint Flags { get; set; } = default!;
    internal string Filename { get; set; } = default!;
}