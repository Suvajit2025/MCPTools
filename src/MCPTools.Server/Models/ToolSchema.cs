namespace MCPTools.Server.Models;

/// <summary>
/// Describes the shape of a tool request or response model.
/// </summary>
public sealed class ToolSchema
{
    /// <summary>
    /// Gets the CLR type represented by the schema.
    /// </summary>
    public required Type SchemaType { get; init; }

    /// <summary>
    /// Gets the schema properties.
    /// </summary>
    public IReadOnlyList<ToolSchemaProperty> Properties { get; init; } = [];
}
