namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents an error that occurs when tool validation fails before execution.
/// </summary>
public class ToolValidationException : MCPToolsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolValidationException"/> class.
    /// </summary>
    public ToolValidationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ToolValidationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolValidationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ToolValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
