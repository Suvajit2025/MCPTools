using MCPTools.Core.Tools.Crud;

namespace MCPTools.Core.Tools.Database;

/// <summary>
/// Represents the outcome of generating CRUD artifacts from a database table.
/// </summary>
public sealed class GenerateCrudFromDatabaseResponse
{
    /// <summary>
    /// Gets the source database table name.
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Gets the generated entity name.
    /// </summary>
    public required string EntityName { get; init; }

    /// <summary>
    /// Gets the CRUD generation response produced by the existing CRUD pipeline.
    /// </summary>
    public required GenerateCrudResponse CrudGeneration { get; init; }
}
