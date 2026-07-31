namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a parameter declared by a method, constructor, or record.
/// </summary>
public sealed class ParameterModel
{
    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the parameter type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the default value declared for the parameter.
    /// </summary>
    public string? DefaultValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether the parameter is optional.
    /// </summary>
    public bool IsOptional { get; init; }

    /// <summary>
    /// Gets a value indicating whether the parameter uses the params modifier.
    /// </summary>
    public bool IsParams { get; init; }

    /// <summary>
    /// Gets the attributes applied to the parameter.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
