using System.Text;
using MCPTools.Core.Configuration;
using MCPTools.Core.Exceptions;
using Microsoft.Extensions.Options;

namespace MCPTools.Core.Services;

/// <summary>
/// Creates generated files on disk.
/// </summary>
public sealed class FileGenerator
{
    private readonly OutputOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileGenerator"/> class.
    /// </summary>
    /// <param name="options">The output configuration options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public FileGenerator(IOptions<OutputOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
    }

    /// <summary>
    /// Generates a single file asynchronously.
    /// </summary>
    /// <param name="outputPath">The path where the file should be written.</param>
    /// <param name="content">The file content.</param>
    /// <param name="overwriteExistingFile">An optional value indicating whether an existing file should be overwritten.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The generated file path.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="outputPath"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <see langword="null"/>.</exception>
    /// <exception cref="FileGenerationException">Thrown when the file cannot be generated.</exception>
    public async Task<string> GenerateFileAsync(
        string outputPath,
        string content,
        bool? overwriteExistingFile = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        ArgumentNullException.ThrowIfNull(content);

        try
        {
            var fullPath = Path.GetFullPath(outputPath);
            var directoryPath = Path.GetDirectoryName(fullPath);

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new FileGenerationException($"The output path '{outputPath}' does not contain a valid directory.");
            }

            if (_options.CreateDirectories)
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (File.Exists(fullPath) && !(overwriteExistingFile ?? _options.OverwriteExistingFiles))
            {
                return fullPath;
            }

            await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8, cancellationToken);
            return fullPath;
        }
        catch (FileGenerationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            throw new FileGenerationException(outputPath, exception, includeOutputPath: true);
        }
    }

    /// <summary>
    /// Generates multiple files asynchronously.
    /// </summary>
    /// <param name="files">The output path and content pairs to generate.</param>
    /// <param name="overwriteExistingFiles">An optional value indicating whether existing files should be overwritten.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The generated file paths.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="files"/> is <see langword="null"/>.</exception>
    public async Task<IReadOnlyList<string>> GenerateFilesAsync(
        IReadOnlyDictionary<string, string> files,
        bool? overwriteExistingFiles = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(files);

        var generatedFiles = new List<string>(files.Count);

        foreach (var file in files)
        {
            var generatedFile = await GenerateFileAsync(
                file.Key,
                file.Value,
                overwriteExistingFiles,
                cancellationToken);

            generatedFiles.Add(generatedFile);
        }

        return generatedFiles;
    }
}
