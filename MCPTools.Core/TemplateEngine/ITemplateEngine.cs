namespace MCPTools.Core.TemplateEngine;

/// <summary>
/// Defines a simple abstraction for rendering text templates using placeholder replacement.
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Renders the specified template using the provided placeholder values.
    /// </summary>
    /// <param name="template">The template text to render.</param>
    /// <param name="values">The placeholder values used during rendering.</param>
    /// <returns>The rendered template text.</returns>
    string Render(
        string template,
        IReadOnlyDictionary<string, string> values);
}
