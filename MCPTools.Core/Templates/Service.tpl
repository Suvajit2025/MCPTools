namespace {{Namespace}}.Application.Services;

using {{Namespace}}.Application.Dtos;
using {{Namespace}}.Application.Mapping;
using {{Namespace}}.Application.Repositories;

/// <summary>
/// Provides application service operations for {{ModelName}}.
/// </summary>
public sealed class {{ModelName}}Service : I{{ModelName}}Service
{
    private readonly I{{ModelName}}Repository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{ModelName}}Service"/> class.
    /// </summary>
    public {{ModelName}}Service(I{{ModelName}}Repository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<{{ModelName}}Response>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select({{ModelName}}Mapper.ToResponse).ToArray();
    }

    /// <inheritdoc />
    public async Task<{{ModelName}}Response?> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync({{PrimaryKey}}, cancellationToken);
        return entity is null ? null : {{ModelName}}Mapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(Create{{ModelName}}Request request, CancellationToken cancellationToken = default)
    {
        var entity = {{ModelName}}Mapper.FromCreateRequest(request);
        return await _repository.InsertAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> UpdateAsync({{PrimaryKeyType}} {{PrimaryKey}}, Update{{ModelName}}Request request, CancellationToken cancellationToken = default)
    {
        var entity = {{ModelName}}Mapper.FromUpdateRequest({{PrimaryKey}}, request);
        return await _repository.UpdateAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync({{PrimaryKey}}, cancellationToken);
    }
}
