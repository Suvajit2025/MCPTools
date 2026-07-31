namespace {{Namespace}}.Infrastructure.Repositories;

using System.Data;
using Dapper;
using {{Namespace}}.Domain.Entities;
using {{Namespace}}.Domain.Repositories;

/// <summary>
/// Provides Dapper-based persistence operations for {{EntityName}}.
/// </summary>
public sealed class {{RepositoryName}} : I{{RepositoryName}}
{
    private readonly IDbConnection _dbConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{RepositoryName}}"/> class.
    /// </summary>
    public {{RepositoryName}}(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<{{EntityName}}>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            "{{StoredProcedureName}}_GetAll",
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var result = await _dbConnection.QueryAsync<{{EntityName}}>(command);
        return result.AsList();
    }

    /// <inheritdoc />
    public async Task<{{EntityName}}?> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@{{PrimaryKey}}", {{PrimaryKey}});

        var command = new CommandDefinition(
            "{{StoredProcedureName}}_GetById",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.QuerySingleOrDefaultAsync<{{EntityName}}>(command);
    }

    /// <inheritdoc />
    public async Task<int> InsertAsync({{EntityName}} entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new DynamicParameters();
{{ParameterList}}

        var command = new CommandDefinition(
            "{{StoredProcedureName}}_Insert",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.ExecuteAsync(command);
    }

    /// <inheritdoc />
    public async Task<int> UpdateAsync({{EntityName}} entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new DynamicParameters();
        parameters.Add("@{{PrimaryKey}}", entity.{{PrimaryKey}});
{{ParameterList}}

        var command = new CommandDefinition(
            "{{StoredProcedureName}}_Update",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.ExecuteAsync(command);
    }

    /// <inheritdoc />
    public async Task<int> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@{{PrimaryKey}}", {{PrimaryKey}});

        var command = new CommandDefinition(
            "{{StoredProcedureName}}_Delete",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.ExecuteAsync(command);
    }
}
