namespace EventPi.Abstractions.IO;

using System;

public static class UnixPath
{
    /// <summary>
    /// Resolves a Unix-style path, including home directory and environment variable substitutions.
    /// </summary>
    /// <param name="path">The path to resolve</param>
    /// <returns>The resolved path with all substitutions applied</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when not running on Linux/WSL</exception>
    /// <exception cref="InvalidOperationException">Thrown when required environment variables are not set</exception>
    public static string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        // First resolve home directory
        path = ResolveHomeDirectory(path);

        // Then resolve environment variables
        path = EnvironmentName.Resolve(path);

        return path;
    }

    private static string ResolveHomeDirectory(string path)
    {
        if (!path.StartsWith("~"))
            return path;

        string homeDir = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(homeDir))
        {
            throw new InvalidOperationException("Could not resolve Linux home directory - HOME environment variable is not set");
        }

        if (!homeDir.StartsWith("/"))
        {
            throw new PlatformNotSupportedException($"Invalid Linux home directory path: {homeDir}. Are you running in Windows instead of WSL/Linux?");
        }

        return path.Replace("~", homeDir);
    }

}