namespace MCPTools.Server.Models;

/// <summary>
/// Describes the shape of a tool request or response model.
/// </summary>
public sealed class ToolSchema
{
    /// <summary>
    /// Gets the schema type name.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// Gets the schema properties.
    /// </summary>
    public IReadOnlyList<ToolSchemaProperty> Properties { get; init; } = [];
}
