namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents an error that occurs when placeholder replacement fails unexpectedly.
/// </summary>
public class PlaceholderReplacementException : MCPToolsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceholderReplacementException"/> class.
    /// </summary>
    public PlaceholderReplacementException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceholderReplacementException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PlaceholderReplacementException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceholderReplacementException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PlaceholderReplacementException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
