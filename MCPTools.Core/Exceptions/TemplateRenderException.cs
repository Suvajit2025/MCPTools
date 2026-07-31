namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents an error that occurs when template rendering fails.
/// </summary>
public class TemplateRenderException : MCPToolsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateRenderException"/> class.
    /// </summary>
    public TemplateRenderException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateRenderException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TemplateRenderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateRenderException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TemplateRenderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateRenderException"/> class for the specified template and original exception.
    /// </summary>
    /// <param name="templateName">The name of the template that failed to render.</param>
    /// <param name="innerException">The original exception that caused rendering to fail.</param>
    public TemplateRenderException(string templateName, Exception innerException, bool includeTemplateName)
        : base($"Failed to render template '{templateName}'.", innerException)
    {
        TemplateName = templateName;
    }

    /// <summary>
    /// Gets the name of the template that failed to render.
    /// </summary>
    public string? TemplateName { get; }
}
