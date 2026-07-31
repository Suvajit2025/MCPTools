namespace MCPTools.Core.Extensions;

/// <summary>
/// Provides path helper extension methods used by MCPTools.
/// </summary>
public static class PathExtensions
{
    /// <summary>
    /// Ensures that the specified directory exists.
    /// </summary>
    /// <param name="directoryPath">The directory path to create when missing.</param>
    /// <returns>The original directory path.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="directoryPath"/> is empty or white space.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is <see langword="null"/>.</exception>
    public static string EnsureDirectoryExists(this string directoryPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        Directory.CreateDirectory(directoryPath);
        return directoryPath;
    }

    /// <summary>
    /// Safely combines the base path with additional path segments.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    /// <param name="paths">The additional path segments.</param>
    /// <returns>The combined path.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="basePath"/> is empty or white space.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="basePath"/> or <paramref name="paths"/> is <see langword="null"/>.</exception>
    public static string CombineSafe(this string basePath, params string[] paths)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(basePath);
        ArgumentNullException.ThrowIfNull(paths);

        return Path.Combine([basePath, .. paths]);
    }

    /// <summary>
    /// Normalizes the specified path to a full path.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized full path.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty or white space.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
    public static string NormalizePath(this string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return Path.GetFullPath(path);
    }
}
