namespace MCPTools.Server.Models;

/// <summary>
/// Describes a property on a tool request or response schema.
/// </summary>
public sealed class ToolSchemaProperty
{
    /// <summary>
    /// Gets the property name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the property type name.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property is required.
    /// </summary>
    public bool IsRequired { get; init; }
}
