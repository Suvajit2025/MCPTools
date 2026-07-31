using System.Data.Common;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Schema;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace MCPTools.Core.Services.Schema;

/// <summary>
/// Creates SQL Server database connections.
/// </summary>
public sealed class SqlConnectionFactory : IDatabaseConnectionFactory
{
    private readonly DatabaseConnectionOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
    /// </summary>
    /// <param name="options">The database connection options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public SqlConnectionFactory(IOptions<DatabaseConnectionOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
    }

    /// <inheritdoc />
    public DbConnection CreateConnection()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = _options.Server,
            InitialCatalog = _options.Database,
            IntegratedSecurity = _options.IntegratedSecurity,
            Encrypt = _options.Encrypt,
            TrustServerCertificate = _options.TrustServerCertificate
        };

        if (!_options.IntegratedSecurity)
        {
            builder.UserID = _options.UserId;
            builder.Password = _options.Password;
        }

        return new SqlConnection(builder.ConnectionString);
    }
}
