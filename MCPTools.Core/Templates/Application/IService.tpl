namespace {{Namespace}}.Application.Services;

using {{Namespace}}.Application.Requests;
using {{Namespace}}.Application.Responses;
using {{Namespace}}.Shared;

/// <summary>
/// Defines application operations for {{EntityName}}.
/// </summary>
public interface I{{ServiceName}}
{
    /// <summary>
    /// Gets all {{PluralEntityName}}.
    /// </summary>
    Task<Result<IReadOnlyList<{{EntityName}}Response>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a {{EntityName}} by its primary key.
    /// </summary>
    Task<Result<{{EntityName}}Response>> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new {{EntityName}}.
    /// </summary>
    Task<Result<{{PrimaryKeyType}}>> CreateAsync(Create{{EntityName}}Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing {{EntityName}}.
    /// </summary>
    Task<Result> UpdateAsync(Update{{EntityName}}Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a {{EntityName}}.
    /// </summary>
    Task<Result> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);
}
