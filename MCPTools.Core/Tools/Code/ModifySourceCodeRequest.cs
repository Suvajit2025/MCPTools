namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents a request to modify a source file by replacing text.
/// </summary>
public sealed class ModifySourceCodeRequest
{
    /// <summary>
    /// Gets the source file path to modify.
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// Gets the text to replace.
    /// </summary>
    public required string SearchText { get; init; }

    /// <summary>
    /// Gets the replacement text.
    /// </summary>
    public required string ReplacementText { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation should only preview the modified content.
    /// </summary>
    public bool PreviewOnly { get; init; } = true;
}
