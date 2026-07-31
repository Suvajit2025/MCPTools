using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Models.Tools;
using Microsoft.Extensions.Logging;

namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Modifies a source file by replacing exact text.
/// </summary>
public sealed class ModifySourceCodeTool : ToolBase<ModifySourceCodeRequest, ModifySourceCodeResponse>
{
    private readonly ILogger<ModifySourceCodeTool> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifySourceCodeTool"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ModifySourceCodeTool(ILogger<ModifySourceCodeTool> logger)
        : base(new ToolMetadata
        {
            Name = "modify-source-code",
            DisplayName = "Modify Source Code",
            Category = ToolMetadataConstants.Categories.Code,
            Version = "1.0.0",
            Description = "Modifies a source file by replacing exact text.",
            Tags =
            [
                ToolMetadataConstants.Tags.Code,
                ToolMetadataConstants.Tags.SourceModification
            ]
        })
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public override async Task<ModifySourceCodeResponse> ExecuteAsync(
        ModifySourceCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FilePath))
        {
            throw new ToolValidationException("FilePath is required.");
        }

        if (string.IsNullOrEmpty(request.SearchText))
        {
            throw new ToolValidationException("SearchText is required.");
        }

        var fullPath = Path.GetFullPath(request.FilePath);

        if (!File.Exists(fullPath))
        {
            throw new ToolValidationException($"Source file '{request.FilePath}' does not exist.");
        }

        _logger.LogInformation("Preparing source modification for {FilePath}.", fullPath);

        var content = await File.ReadAllTextAsync(fullPath, cancellationToken);

        if (!content.Contains(request.SearchText, StringComparison.Ordinal))
        {
            return new ModifySourceCodeResponse
            {
                Success = true,
                Modified = false,
                FilePath = fullPath,
                Message = "Search text was not found."
            };
        }

        var modifiedContent = content.Replace(request.SearchText, request.ReplacementText, StringComparison.Ordinal);

        if (!request.PreviewOnly)
        {
            await File.WriteAllTextAsync(fullPath, modifiedContent, cancellationToken);
        }

        return new ModifySourceCodeResponse
        {
            Success = true,
            Modified = !request.PreviewOnly,
            FilePath = fullPath,
            PreviewContent = request.PreviewOnly ? modifiedContent : null,
            Message = request.PreviewOnly
                ? "Source modification preview created."
                : "Source file modified successfully."
        };
    }
}
