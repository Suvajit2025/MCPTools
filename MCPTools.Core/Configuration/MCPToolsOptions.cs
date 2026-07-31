namespace MCPTools.Core.Configuration;

/// <summary>
/// Represents the root configuration options for MCPTools.
/// </summary>
public sealed class MCPToolsOptions
{
    /// <summary>
    /// Gets the template engine configuration options.
    /// </summary>
    public TemplateOptions Templates { get; init; } = new();

    /// <summary>
    /// Gets the code generator configuration options.
    /// </summary>
    public GeneratorOptions Generator { get; init; } = new();

    /// <summary>
    /// Gets the generated output configuration options.
    /// </summary>
    public OutputOptions Output { get; init; } = new();

    /// <summary>
    /// Gets the logging configuration options.
    /// </summary>
    public LoggingOptions Logging { get; init; } = new();
}
