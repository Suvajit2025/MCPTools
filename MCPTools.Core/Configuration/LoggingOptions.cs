namespace MCPTools.Core.Configuration;

/// <summary>
/// Represents configuration options for MCPTools logging behavior.
/// </summary>
public sealed class LoggingOptions
{
    /// <summary>
    /// Gets a value indicating whether framework logging is enabled.
    /// </summary>
    public bool EnableLogging { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether generated files should be logged.
    /// </summary>
    public bool LogGeneratedFiles { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether template loading should be logged.
    /// </summary>
    public bool LogTemplateLoading { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether execution time should be logged.
    /// </summary>
    public bool LogExecutionTime { get; init; } = true;
}
