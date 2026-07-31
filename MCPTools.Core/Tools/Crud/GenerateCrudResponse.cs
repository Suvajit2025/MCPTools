namespace MCPTools.Core.Tools.Crud;

/// <summary>
/// Represents the outcome of CRUD generation.
/// </summary>
public sealed class GenerateCrudResponse
{
    /// <summary>
    /// Gets a value indicating whether CRUD generation completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the files generated during CRUD generation.
    /// </summary>
    public IReadOnlyList<string> GeneratedFiles { get; init; } = [];

    /// <summary>
    /// Gets the files skipped during CRUD generation.
    /// </summary>
    public IReadOnlyList<string> SkippedFiles { get; init; } = [];

    /// <summary>
    /// Gets the errors produced during CRUD generation.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>
    /// Gets the elapsed CRUD generation time.
    /// </summary>
    public TimeSpan ElapsedTime { get; init; }

    /// <summary>
    /// Gets a human-readable summary of the CRUD generation result.
    /// </summary>
    public string? Message { get; init; }
}
