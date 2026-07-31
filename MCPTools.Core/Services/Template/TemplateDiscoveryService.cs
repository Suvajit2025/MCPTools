using MCPTools.Core.Configuration;
using MCPTools.Core.Models.Template;
using Microsoft.Extensions.Options;

namespace MCPTools.Core.Services;

/// <summary>
/// Discovers template files from configured template folders.
/// </summary>
public sealed class TemplateDiscoveryService
{
    private readonly TemplateOptions _options;
    private readonly object _syncRoot = new();
    private IReadOnlyList<TemplateDefinition>? _templateCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateDiscoveryService"/> class.
    /// </summary>
    /// <param name="options">The template configuration options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public TemplateDiscoveryService(IOptions<TemplateOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
    }

    /// <summary>
    /// Gets all discovered templates.
    /// </summary>
    /// <returns>A read-only collection of template metadata.</returns>
    public IReadOnlyList<TemplateDefinition> GetAllTemplates()
    {
        if (_options.CacheTemplates && _templateCache is not null)
        {
            return _templateCache;
        }

        lock (_syncRoot)
        {
            if (_options.CacheTemplates && _templateCache is not null)
            {
                return _templateCache;
            }

            var templates = DiscoverTemplates();

            if (_options.CacheTemplates)
            {
                _templateCache = templates;
            }

            return templates;
        }
    }

    /// <summary>
    /// Gets a template by file name or relative path.
    /// </summary>
    /// <param name="templateName">The template file name or relative path.</param>
    /// <returns>The template metadata when found; otherwise, <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="templateName"/> is empty or whitespace.</exception>
    public TemplateDefinition? GetTemplate(string templateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        var normalizedTemplateName = NormalizeRelativePath(templateName);

        return GetAllTemplates().FirstOrDefault(template =>
            string.Equals(template.Name, templateName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(NormalizeRelativePath(template.RelativePath), normalizedTemplateName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether a template exists.
    /// </summary>
    /// <param name="templateName">The template file name or relative path.</param>
    /// <returns><see langword="true"/> when the template exists; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="templateName"/> is empty or whitespace.</exception>
    public bool TemplateExists(string templateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        return GetTemplate(templateName) is not null;
    }

    private IReadOnlyList<TemplateDefinition> DiscoverTemplates()
    {
        var templateRoot = ResolveTemplateRoot();

        if (!Directory.Exists(templateRoot))
        {
            return [];
        }

        var searchPattern = $"*{_options.TemplateExtension}";

        return Directory
            .EnumerateFiles(templateRoot, searchPattern, SearchOption.AllDirectories)
            .Select(path => CreateTemplateDefinition(templateRoot, path))
            .OrderBy(template => template.RelativePath, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private string ResolveTemplateRoot()
    {
        return Path.IsPathRooted(_options.TemplateRoot)
            ? Path.GetFullPath(_options.TemplateRoot)
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _options.TemplateRoot));
    }

    private static TemplateDefinition CreateTemplateDefinition(string templateRoot, string templatePath)
    {
        var relativePath = Path.GetRelativePath(templateRoot, templatePath);
        var category = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).FirstOrDefault();
        var fileInfo = new FileInfo(templatePath);

        return new TemplateDefinition
        {
            Name = Path.GetFileName(templatePath),
            FullPath = templatePath,
            RelativePath = relativePath,
            Category = category,
            LastModifiedUtc = fileInfo.LastWriteTimeUtc
        };
    }

    private static string NormalizeRelativePath(string path)
    {
        return path.Replace('\\', '/');
    }
}
