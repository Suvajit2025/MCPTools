namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents the base exception for all MCPTools framework-specific errors.
/// </summary>
public class MCPToolsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MCPToolsException"/> class.
    /// </summary>
    public MCPToolsException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MCPToolsException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MCPToolsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MCPToolsException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MCPToolsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
