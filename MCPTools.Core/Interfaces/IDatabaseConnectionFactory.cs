using System.Data.Common;

namespace MCPTools.Core.Interfaces;

/// <summary>
/// Defines a factory for creating database connections.
/// </summary>
public interface IDatabaseConnectionFactory
{
    /// <summary>
    /// Creates a new database connection instance.
    /// </summary>
    /// <returns>A new database connection.</returns>
    DbConnection CreateConnection();
}
