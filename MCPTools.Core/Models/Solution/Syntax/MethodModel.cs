namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a method declaration discovered in source code.
/// </summary>
public sealed class MethodModel
{
    /// <summary>
    /// Gets the method name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the method return type.
    /// </summary>
    public string? ReturnType { get; init; }

    /// <summary>
    /// Gets the method accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets a value indicating whether the method is asynchronous.
    /// </summary>
    public bool IsAsync { get; init; }

    /// <summary>
    /// Gets a value indicating whether the method is static.
    /// </summary>
    public bool IsStatic { get; init; }

    /// <summary>
    /// Gets the method parameters.
    /// </summary>
    public IReadOnlyList<ParameterModel> Parameters { get; init; } = [];

    /// <summary>
    /// Gets the attributes applied to the method.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
