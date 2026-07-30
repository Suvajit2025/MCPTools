using System.Text;

namespace MCPTools.Core.TemplateEngine;

/// <summary>
/// Loads template files from disk.
/// </summary>
public sealed class FileTemplateLoader
{
    /// <summary>
    /// Loads a template file using UTF-8 encoding.
    /// </summary>
    /// <param name="templatePath">The path of the template file to load.</param>
    /// <returns>The template file content.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="templatePath"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="templatePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the template file does not exist.</exception>
    public string Load(string templatePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templatePath);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                $"The template file '{templatePath}' was not found.",
                templatePath);
        }

        return File.ReadAllText(templatePath, Encoding.UTF8);
    }
}
