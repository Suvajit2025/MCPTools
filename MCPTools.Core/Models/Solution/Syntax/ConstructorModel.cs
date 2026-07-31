namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a constructor declaration discovered in source code.
/// </summary>
public sealed class ConstructorModel
{
    /// <summary>
    /// Gets the constructor name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the constructor accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets a value indicating whether the constructor is static.
    /// </summary>
    public bool IsStatic { get; init; }

    /// <summary>
    /// Gets the constructor parameters.
    /// </summary>
    public IReadOnlyList<ParameterModel> Parameters { get; init; } = [];

    /// <summary>
    /// Gets the attributes applied to the constructor.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
