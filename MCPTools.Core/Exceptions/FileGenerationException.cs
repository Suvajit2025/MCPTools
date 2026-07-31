namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents an error that occurs when generated files cannot be written to disk.
/// </summary>
public class FileGenerationException : MCPToolsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileGenerationException"/> class.
    /// </summary>
    public FileGenerationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileGenerationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FileGenerationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileGenerationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public FileGenerationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileGenerationException"/> class for the specified output path.
    /// </summary>
    /// <param name="outputPath">The output path that could not be written.</param>
    /// <param name="innerException">The exception that caused file generation to fail.</param>
    public FileGenerationException(string outputPath, Exception innerException, bool includeOutputPath)
        : base($"Generated files could not be written to '{outputPath}'.", innerException)
    {
        OutputPath = outputPath;
    }

    /// <summary>
    /// Gets the output path that could not be written.
    /// </summary>
    public string? OutputPath { get; }
}
