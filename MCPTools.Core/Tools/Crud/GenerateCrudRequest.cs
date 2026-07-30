namespace MCPTools.Core.Tools.Crud;

/// <summary>
/// Represents the information required to generate CRUD code for a single database table.
/// </summary>
public sealed class GenerateCrudRequest
{
    /// <summary>
    /// Gets the absolute path of the target solution.
    /// </summary>
    public required string ProjectPath { get; init; }

    /// <summary>
    /// Gets the directory where generated files will be written.
    /// </summary>
    public required string OutputPath { get; init; }

    /// <summary>
    /// Gets the root namespace of the generated project.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// Gets the database table name.
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Gets the primary key column.
    /// </summary>
    public required string PrimaryKey { get; init; }

    /// <summary>
    /// Gets the entity or model name.
    /// </summary>
    public required string ModelName { get; init; }

    /// <summary>
    /// Gets a value indicating whether a controller should be generated.
    /// </summary>
    public bool GenerateController { get; init; }

    /// <summary>
    /// Gets a value indicating whether a service should be generated.
    /// </summary>
    public bool GenerateService { get; init; }

    /// <summary>
    /// Gets a value indicating whether a repository should be generated.
    /// </summary>
    public bool GenerateRepository { get; init; }

    /// <summary>
    /// Gets a value indicating whether interfaces should be generated.
    /// </summary>
    public bool GenerateInterface { get; init; }

    /// <summary>
    /// Gets a value indicating whether DTOs should be generated.
    /// </summary>
    public bool GenerateDto { get; init; }

    /// <summary>
    /// Gets a value indicating whether existing generated files should be overwritten.
    /// </summary>
    public bool OverwriteExistingFiles { get; init; }
}
