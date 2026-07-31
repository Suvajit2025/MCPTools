namespace MCPTools.Core.Models.Generation;

/// <summary>
/// Represents the root request for CRUD generation.
/// </summary>
public sealed class CrudGenerationRequest
{
    /// <summary>
    /// Gets the entity definition used for CRUD generation.
    /// </summary>
    public required EntityDefinition Entity { get; init; }

    /// <summary>
    /// Gets the output directory for generated artifacts.
    /// </summary>
    public required string OutputDirectory { get; init; }

    /// <summary>
    /// Gets the template directory used for generation.
    /// </summary>
    public required string TemplateDirectory { get; init; }

    /// <summary>
    /// Gets a value indicating whether existing files should be overwritten.
    /// </summary>
    public bool OverwriteExistingFiles { get; init; }

    /// <summary>
    /// Gets a value indicating whether SQL artifacts should be generated.
    /// </summary>
    public bool GenerateSql { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether API artifacts should be generated.
    /// </summary>
    public bool GenerateApi { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether Angular artifacts should be generated.
    /// </summary>
    public bool GenerateAngular { get; init; }

    /// <summary>
    /// Gets a value indicating whether React artifacts should be generated.
    /// </summary>
    public bool GenerateReact { get; init; }

    /// <summary>
    /// Gets a value indicating whether Blazor artifacts should be generated.
    /// </summary>
    public bool GenerateBlazor { get; init; }
}
