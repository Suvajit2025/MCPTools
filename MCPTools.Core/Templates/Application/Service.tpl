namespace {{Namespace}}.Application.Services;

using {{Namespace}}.Application.Mapping;
using {{Namespace}}.Application.Requests;
using {{Namespace}}.Application.Responses;
using {{Namespace}}.Domain.Repositories;
using {{Namespace}}.Shared;

/// <summary>
/// Provides application operations for {{EntityName}}.
/// </summary>
public sealed class {{ServiceName}} : I{{ServiceName}}
{
    private readonly I{{RepositoryName}} _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{ServiceName}}"/> class.
    /// </summary>
    public {{ServiceName}}(I{{RepositoryName}} repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<{{EntityName}}Response>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var response = entities.Select({{EntityName}}Mapper.ToResponse).ToArray();
        return Result<IReadOnlyList<{{EntityName}}Response>>.Ok(response);
    }

    /// <inheritdoc />
    public async Task<Result<{{EntityName}}Response>> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync({{PrimaryKey}}, cancellationToken);
        return entity is null
            ? Result<{{EntityName}}Response>.Fail("{{EntityName}} was not found.")
            : Result<{{EntityName}}Response>.Ok({{EntityName}}Mapper.ToResponse(entity));
    }

    /// <inheritdoc />
    public async Task<Result<{{PrimaryKeyType}}>> CreateAsync(Create{{EntityName}}Request request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = {{EntityName}}Mapper.FromCreateRequest(request);
        await _repository.InsertAsync(entity, cancellationToken);
        return Result<{{PrimaryKeyType}}>.Ok(entity.{{PrimaryKey}});
    }

    /// <inheritdoc />
    public async Task<Result> UpdateAsync(Update{{EntityName}}Request request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = {{EntityName}}Mapper.FromUpdateRequest(request);
        var affectedRows = await _repository.UpdateAsync(entity, cancellationToken);
        return affectedRows > 0 ? Result.Ok() : Result.Fail("{{EntityName}} was not found.");
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        var affectedRows = await _repository.DeleteAsync({{PrimaryKey}}, cancellationToken);
        return affectedRows > 0 ? Result.Ok() : Result.Fail("{{EntityName}} was not found.");
    }
}
