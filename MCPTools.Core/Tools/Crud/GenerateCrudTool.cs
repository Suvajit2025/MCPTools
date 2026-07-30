using MCPTools.Core.Models.Tools;

namespace MCPTools.Core.Tools.Crud;

/// <summary>
/// Simulates CRUD source file generation for a single entity.
/// </summary>
public sealed class GenerateCrudTool : ToolBase<GenerateCrudRequest, GenerateCrudResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateCrudTool"/> class.
    /// </summary>
    public GenerateCrudTool()
        : base(new ToolMetadata
        {
            Name = "generate-crud",
            DisplayName = "Generate CRUD",
            Category = "Generation",
            Version = "1.0.0",
            Description = "Generates CRUD source code for a database entity."
        })
    {
    }

    /// <inheritdoc />
    public override Task<GenerateCrudResponse> ExecuteAsync(
        GenerateCrudRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        cancellationToken.ThrowIfCancellationRequested();

        var modelName = request.ModelName;
        var generatedFiles = new[]
        {
            $"Models/{modelName}.cs",
            $"Repositories/I{modelName}Repository.cs",
            $"Repositories/{modelName}Repository.cs",
            $"Services/I{modelName}Service.cs",
            $"Services/{modelName}Service.cs",
            $"Controllers/{modelName}Controller.cs"
        };

        var response = new GenerateCrudResponse
        {
            Success = true,
            GeneratedFiles = generatedFiles,
            Message = $"CRUD generation simulated successfully for '{modelName}'."
        };

        return Task.FromResult(response);
    }

    private static void ValidateRequest(GenerateCrudRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ProjectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.OutputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Namespace);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PrimaryKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ModelName);
    }
}
