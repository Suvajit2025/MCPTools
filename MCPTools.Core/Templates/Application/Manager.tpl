namespace {{Namespace}}.Application.Managers;

using {{Namespace}}.Application.Requests;
using {{Namespace}}.Application.Responses;
using {{Namespace}}.Application.Services;
using {{Namespace}}.Shared;

/// <summary>
/// Coordinates application workflows for {{EntityName}}.
/// </summary>
public sealed class {{ManagerName}} : I{{ManagerName}}
{
    private readonly I{{ServiceName}} _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{ManagerName}}"/> class.
    /// </summary>
    public {{ManagerName}}(I{{ServiceName}} service)
    {
        _service = service;
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<{{EntityName}}Response>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _service.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<{{EntityName}}Response>> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        return _service.GetByIdAsync({{PrimaryKey}}, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<{{PrimaryKeyType}}>> CreateAsync(Create{{EntityName}}Request request, CancellationToken cancellationToken = default)
    {
        return _service.CreateAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result> UpdateAsync(Update{{EntityName}}Request request, CancellationToken cancellationToken = default)
    {
        return _service.UpdateAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        return _service.DeleteAsync({{PrimaryKey}}, cancellationToken);
    }
}
