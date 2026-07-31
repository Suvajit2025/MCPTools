namespace MCPTools.Core.Models.Generation;

/// <summary>
/// Represents the result of CRUD generation.
/// </summary>
public sealed class CrudGenerationResult
{
    private readonly List<string> _generatedFiles = [];
    private readonly List<string> _skippedFiles = [];
    private readonly List<string> _errors = [];

    /// <summary>
    /// Gets a value indicating whether generation completed successfully.
    /// </summary>
    public bool Success => _errors.Count == 0;

    /// <summary>
    /// Gets the files generated during generation.
    /// </summary>
    public IReadOnlyList<string> GeneratedFiles => _generatedFiles;

    /// <summary>
    /// Gets the files skipped during generation.
    /// </summary>
    public IReadOnlyList<string> SkippedFiles => _skippedFiles;

    /// <summary>
    /// Gets the errors produced during generation.
    /// </summary>
    public IReadOnlyList<string> Errors => _errors;

    /// <summary>
    /// Gets or sets the elapsed generation time.
    /// </summary>
    public TimeSpan ElapsedTime { get; set; }

    /// <summary>
    /// Adds a generated file path to the result.
    /// </summary>
    /// <param name="filePath">The generated file path.</param>
    public void AddGenerated(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _generatedFiles.Add(filePath);
    }

    /// <summary>
    /// Adds a skipped file path to the result.
    /// </summary>
    /// <param name="filePath">The skipped file path.</param>
    public void AddSkipped(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _skippedFiles.Add(filePath);
    }

    /// <summary>
    /// Adds an error message to the result.
    /// </summary>
    /// <param name="error">The error message.</param>
    public void AddError(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        _errors.Add(error);
    }
}
