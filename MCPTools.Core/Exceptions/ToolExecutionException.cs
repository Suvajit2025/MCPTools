namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents an error that occurs when tool execution fails.
/// </summary>
public class ToolExecutionException : MCPToolsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolExecutionException"/> class.
    /// </summary>
    public ToolExecutionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolExecutionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ToolExecutionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolExecutionException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ToolExecutionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolExecutionException"/> class for the specified tool.
    /// </summary>
    /// <param name="toolName">The name of the tool that failed during execution.</param>
    /// <param name="innerException">The exception that caused tool execution to fail.</param>
    public ToolExecutionException(string toolName, Exception innerException, bool includeToolName)
        : base($"Execution failed for tool '{toolName}'.", innerException)
    {
        ToolName = toolName;
    }

    /// <summary>
    /// Gets the name of the tool that failed during execution.
    /// </summary>
    public string? ToolName { get; }
}
