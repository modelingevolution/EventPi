namespace NetworkManager.DBus;

class PropertyChanges<TProperties>
{
    internal PropertyChanges(TProperties properties, string[] invalidated, string[] changed)
        => (Properties, Invalidated, Changed) = (properties, invalidated, changed);
    internal TProperties Properties { get; }
    internal string[] Invalidated { get; }
    internal string[] Changed { get; }
    internal bool HasChanged(string property) => Array.IndexOf(Changed, property) != -1;
    internal bool IsInvalidated(string property) => Array.IndexOf(Invalidated, property) != -1;
}