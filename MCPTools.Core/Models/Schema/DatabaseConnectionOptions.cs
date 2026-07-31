namespace MCPTools.Core.Models.Schema;

/// <summary>
/// Represents SQL Server database connection options used for schema discovery.
/// </summary>
public sealed class DatabaseConnectionOptions
{
    /// <summary>
    /// Gets the database server name or address.
    /// </summary>
    public string? Server { get; init; }

    /// <summary>
    /// Gets the database name.
    /// </summary>
    public string? Database { get; init; }

    /// <summary>
    /// Gets the database user identifier.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets the database password.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets a value indicating whether integrated security should be used.
    /// </summary>
    public bool IntegratedSecurity { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the database connection should be encrypted.
    /// </summary>
    public bool Encrypt { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the server certificate should be trusted.
    /// </summary>
    public bool TrustServerCertificate { get; init; }
}
