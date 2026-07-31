namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents an attribute applied to a source code declaration.
/// </summary>
public sealed class AttributeModel
{
    /// <summary>
    /// Gets the attribute name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the attribute argument values.
    /// </summary>
    public IReadOnlyList<string> Arguments { get; init; } = [];
}
