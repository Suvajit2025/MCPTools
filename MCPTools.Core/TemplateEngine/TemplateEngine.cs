namespace MCPTools.Core.TemplateEngine;

/// <summary>
/// Provides simple placeholder-based template rendering.
/// </summary>
public sealed class TemplateEngine : ITemplateEngine
{
    /// <inheritdoc />
    public string Render(
        string template,
        IReadOnlyDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(values);

        var renderedTemplate = template;

        foreach (var value in values)
        {
            var placeholder = $"{{{{{value.Key}}}}}";
            renderedTemplate = renderedTemplate.Replace(
                placeholder,
                value.Value,
                StringComparison.Ordinal);
        }

        return renderedTemplate;
    }
}
