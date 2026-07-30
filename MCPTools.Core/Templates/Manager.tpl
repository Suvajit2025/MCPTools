namespace {{Namespace}}.Application.Managers;

using {{Namespace}}.Application.Dtos;
using {{Namespace}}.Application.Services;

/// <summary>
/// Coordinates application workflows for {{ModelName}}.
/// </summary>
public sealed class {{ModelName}}Manager : I{{ModelName}}Manager
{
    private readonly I{{ModelName}}Service _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{ModelName}}Manager"/> class.
    /// </summary>
    public {{ModelName}}Manager(I{{ModelName}}Service service)
    {
        _service = service;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<{{ModelName}}Response>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _service.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<{{ModelName}}Response?> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        return _service.GetByIdAsync({{PrimaryKey}}, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CreateAsync(Create{{ModelName}}Request request, CancellationToken cancellationToken = default)
    {
        return _service.CreateAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> UpdateAsync({{PrimaryKeyType}} {{PrimaryKey}}, Update{{ModelName}}Request request, CancellationToken cancellationToken = default)
    {
        return _service.UpdateAsync({{PrimaryKey}}, request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        return _service.DeleteAsync({{PrimaryKey}}, cancellationToken);
    }
}
