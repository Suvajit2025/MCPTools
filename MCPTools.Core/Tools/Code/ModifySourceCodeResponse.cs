namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents the result of modifying source code.
/// </summary>
public sealed class ModifySourceCodeResponse
{
    /// <summary>
    /// Gets a value indicating whether the request completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets a value indicating whether the file was modified.
    /// </summary>
    public bool Modified { get; init; }

    /// <summary>
    /// Gets the source file path.
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// Gets the preview content when preview mode is enabled.
    /// </summary>
    public string? PreviewContent { get; init; }

    /// <summary>
    /// Gets a human-readable summary of the operation.
    /// </summary>
    public string? Message { get; init; }
}
