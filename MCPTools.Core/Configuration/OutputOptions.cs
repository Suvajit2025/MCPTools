namespace MCPTools.Core.Configuration;

/// <summary>
/// Represents configuration options for generated output.
/// </summary>
public sealed class OutputOptions
{
    /// <summary>
    /// Gets the directory where generated files should be written.
    /// </summary>
    public string OutputDirectory { get; init; } = "Generated";

    /// <summary>
    /// Gets a value indicating whether output directories should be created automatically.
    /// </summary>
    public bool CreateDirectories { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether existing files should be overwritten.
    /// </summary>
    public bool OverwriteExistingFiles { get; init; }

    /// <summary>
    /// Gets a value indicating whether generated C# files should use file-scoped namespaces.
    /// </summary>
    public bool UseFileScopedNamespaces { get; init; } = true;
}
