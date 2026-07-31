namespace MCPTools.Core.Configuration;

/// <summary>
/// Represents configuration options for code generation behavior.
/// </summary>
public sealed class GeneratorOptions
{
    /// <summary>
    /// Gets a value indicating whether existing files should be overwritten by default.
    /// </summary>
    public bool OverwriteExistingFiles { get; init; }

    /// <summary>
    /// Gets a value indicating whether XML documentation should be generated.
    /// </summary>
    public bool GenerateXmlDocumentation { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether asynchronous methods should be generated.
    /// </summary>
    public bool GenerateAsyncMethods { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether cancellation tokens should be generated for asynchronous methods.
    /// </summary>
    public bool GenerateCancellationTokens { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether nullable reference types should be generated.
    /// </summary>
    public bool GenerateNullableReferenceTypes { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether region directives should be generated.
    /// </summary>
    public bool GenerateRegions { get; init; }
}
