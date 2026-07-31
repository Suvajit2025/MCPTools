namespace MCPTools.Core.Exceptions;

/// <summary>
/// Represents an error that occurs when a requested template file cannot be found.
/// </summary>
public class TemplateNotFoundException : MCPToolsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateNotFoundException"/> class.
    /// </summary>
    public TemplateNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TemplateNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateNotFoundException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TemplateNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateNotFoundException"/> class for the specified template name.
    /// </summary>
    /// <param name="templateName">The name of the missing template.</param>
    public TemplateNotFoundException(string templateName, bool includeTemplateName)
        : base($"The template '{templateName}' could not be found.")
    {
        TemplateName = templateName;
    }

    /// <summary>
    /// Gets the name of the missing template.
    /// </summary>
    public string? TemplateName { get; }
}
