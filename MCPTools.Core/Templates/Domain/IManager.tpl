namespace {{Namespace}}.Domain.Managers;

using {{Namespace}}.Domain.Entities;

/// <summary>
/// Defines domain-level workflow operations for {{EntityName}}.
/// </summary>
public interface I{{ManagerName}}
{
    /// <summary>
    /// Applies domain rules before a {{EntityName}} is created.
    /// </summary>
    Task<{{EntityName}}> PrepareForCreateAsync({{EntityName}} entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies domain rules before a {{EntityName}} is updated.
    /// </summary>
    Task<{{EntityName}}> PrepareForUpdateAsync({{EntityName}} entity, CancellationToken cancellationToken = default);
}
