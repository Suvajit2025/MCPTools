using System.Text;

namespace MCPTools.Core.Extensions;

/// <summary>
/// Provides string helper extension methods used by MCPTools.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Determines whether the specified value is <see langword="null"/>, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string value to inspect.</param>
    /// <returns><see langword="true"/> when the value is null, empty, or white space; otherwise, <see langword="false"/>.</returns>
    public static bool IsNullOrWhiteSpaceEx(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Converts the specified value into a safe file name.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A file-name-safe string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToSafeFileName(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var invalidCharacters = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            builder.Append(invalidCharacters.Contains(character) ? '_' : character);
        }

        return builder.ToString().Trim();
    }

    /// <summary>
    /// Converts the specified value to PascalCase.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The PascalCase representation of the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToPascalCase(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var words = SplitWords(value);
        var builder = new StringBuilder();

        foreach (var word in words)
        {
            builder.Append(char.ToUpperInvariant(word[0]));

            if (word.Length > 1)
            {
                builder.Append(word[1..].ToLowerInvariant());
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Converts the specified value to camelCase.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The camelCase representation of the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToCamelCase(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var pascalCase = value.ToPascalCase();

        if (pascalCase.Length == 0)
        {
            return pascalCase;
        }

        return char.ToLowerInvariant(pascalCase[0]) + pascalCase[1..];
    }

    private static IReadOnlyList<string> SplitWords(string value)
    {
        return value
            .Split([' ', '-', '_', '.', '/'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(word => word.Length > 0)
            .ToArray();
    }
}
