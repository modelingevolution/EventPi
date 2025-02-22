namespace EventPi.Abstractions.IO;

public static class UnixDirectory
{
    public static void Create(string path, string other, params string[] rest)
    {
        string fullPath = Path.Combine(path, other, Path.Combine(rest));
        Create(fullPath);
    }
    public static void Create(string path, int maxDepth = 32)
    {
        // Normalize path separators and trim trailing slashes
        path = path.Replace('\\', '/').TrimEnd('/');
        if (string.IsNullOrEmpty(path)) return;

        // If directory already exists, we're done
        if (Directory.Exists(path)) return;

        var stack = new Stack<string>();
        var current = path;

        // Check each level up to max depth
        for (int depth = 0; depth < maxDepth; depth++)
        {
            if (string.IsNullOrEmpty(current) || Directory.Exists(current))
                break;

            stack.Push(current);
            current = Path.GetDirectoryName(current)?.Replace('\\', '/');
        }

        // If we still have a path but hit max depth, throw
        if (!string.IsNullOrEmpty(current) && !Directory.Exists(current))
            throw new IOException($"Directory structure exceeds maximum depth of {maxDepth}");

        // Create directories from bottom up
        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            Directory.CreateDirectory(dir);
        }
    }
}