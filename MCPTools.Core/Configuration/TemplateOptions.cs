namespace MCPTools.Core.Configuration;

/// <summary>
/// Represents configuration options for template loading and rendering.
/// </summary>
public sealed class TemplateOptions
{
    /// <summary>
    /// Gets the root directory where template files are located.
    /// </summary>
    public string TemplateRoot { get; init; } = "Templates";

    /// <summary>
    /// Gets a value indicating whether embedded templates should be used.
    /// </summary>
    public bool UseEmbeddedTemplates { get; init; } = true;

    /// <summary>
    /// Gets the template file extension.
    /// </summary>
    public string TemplateExtension { get; init; } = ".tpl";

    /// <summary>
    /// Gets a value indicating whether templates should be cached after loading.
    /// </summary>
    public bool CacheTemplates { get; init; } = true;
}
