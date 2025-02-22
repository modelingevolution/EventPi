using System.Text.RegularExpressions;

namespace EventPi.Abstractions.IO;

public static class EnvironmentName
{
    private static readonly Regex VariablePattern = new(@"\$\{([^}]+)\}|\$([a-zA-Z_][a-zA-Z0-9_]*)");

    /// <summary>
    /// Resolves environment variables in the given text, supporting both ${VAR} and $VAR formats.
    /// </summary>
    /// <param name="text">The text containing environment variable references</param>
    /// <param name="throwOnMissing">If true, throws when an environment variable is not found. If false, leaves unresolved variables as-is.</param>
    /// <returns>The text with environment variables resolved</returns>
    /// <exception cref="InvalidOperationException">Thrown when a required environment variable is not set and throwOnMissing is true</exception>
    public static string Resolve(string text, bool throwOnMissing = true)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return VariablePattern.Replace(text, match =>
        {
            // Get the variable name from either ${VAR} or $VAR format
            string varName = match.Groups[1].Success
                ? match.Groups[1].Value
                : match.Groups[2].Value;

            string? value = Environment.GetEnvironmentVariable(varName);
            if (string.IsNullOrEmpty(value))
            {
                if (throwOnMissing)
                    throw new InvalidOperationException($"Environment variable '{varName}' is not set");
                return match.Value; // Keep the original text if variable not found
            }

            return value;
        });
    }
}