namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents an error that occurs when a template contains an unknown or invalid placeholder.
/// </summary>
public class InvalidPlaceholderException : MCPToolsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPlaceholderException"/> class.
    /// </summary>
    public InvalidPlaceholderException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPlaceholderException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidPlaceholderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPlaceholderException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InvalidPlaceholderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPlaceholderException"/> class for the specified placeholder.
    /// </summary>
    /// <param name="placeholderName">The name of the invalid placeholder.</param>
    public InvalidPlaceholderException(string placeholderName, bool includePlaceholderName)
        : base($"The placeholder '{placeholderName}' is invalid or unsupported.")
    {
        PlaceholderName = placeholderName;
    }

    /// <summary>
    /// Gets the name of the invalid placeholder.
    /// </summary>
    public string? PlaceholderName { get; }
}
