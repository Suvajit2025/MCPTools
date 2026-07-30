namespace {{Namespace}}.Infrastructure.Repositories;

using System.Data;
using Dapper;
using {{Namespace}}.Application.Repositories;
using {{Namespace}}.Domain.Entities;

/// <summary>
/// Provides Dapper-based repository operations for {{ModelName}}.
/// </summary>
public sealed class {{ModelName}}Repository : I{{ModelName}}Repository
{
    private readonly IDbConnection _dbConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{ModelName}}Repository"/> class.
    /// </summary>
    public {{ModelName}}Repository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<{{ModelName}}>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            "{{StoredProcedure}}_GetAll",
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var result = await _dbConnection.QueryAsync<{{ModelName}}>(command);
        return result.AsList();
    }

    /// <inheritdoc />
    public async Task<{{ModelName}}?> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@{{PrimaryKey}}", {{PrimaryKey}});

        var command = new CommandDefinition(
            "{{StoredProcedure}}_GetById",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.QuerySingleOrDefaultAsync<{{ModelName}}>(command);
    }

    /// <inheritdoc />
    public async Task<int> InsertAsync({{ModelName}} entity, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
{{ParameterList}}

        var command = new CommandDefinition(
            "{{StoredProcedure}}_Insert",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.ExecuteAsync(command);
    }

    /// <inheritdoc />
    public async Task<int> UpdateAsync({{ModelName}} entity, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@{{PrimaryKey}}", entity.{{PrimaryKey}});
{{ParameterList}}

        var command = new CommandDefinition(
            "{{StoredProcedure}}_Update",
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
            "{{StoredProcedure}}_Delete",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.ExecuteAsync(command);
    }
}
