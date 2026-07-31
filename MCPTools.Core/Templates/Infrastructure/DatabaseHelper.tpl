namespace {{Namespace}}.Infrastructure.Data;

using System.Data;

/// <summary>
/// Provides database helper operations for {{ProjectName}}.
/// </summary>
public sealed class DatabaseHelper
{
    private readonly {{ProjectName}}DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseHelper"/> class.
    /// </summary>
    public DatabaseHelper({{ProjectName}}DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates and opens a database connection.
    /// </summary>
    public async Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.CreateConnection();

        if (connection is System.Data.Common.DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
            return connection;
        }

        connection.Open();
        return connection;
    }
}
