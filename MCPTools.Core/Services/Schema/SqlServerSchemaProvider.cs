using System.Data;
using System.Data.Common;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Schema;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MCPTools.Core.Services.Schema;

/// <summary>
/// Reads SQL Server schema metadata using ADO.NET.
/// </summary>
public sealed class SqlServerSchemaProvider : ISchemaProvider
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<SqlServerSchemaProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerSchemaProvider"/> class.
    /// </summary>
    /// <param name="connectionFactory">The database connection factory.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public SqlServerSchemaProvider(
        IDatabaseConnectionFactory connectionFactory,
        ILogger<SqlServerSchemaProvider> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<TableSchema>> GetTablesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var tableKeys = await ReadTableKeysAsync(connection, cancellationToken);
            var tables = new List<TableSchema>(tableKeys.Count);

            foreach (var tableKey in tableKeys)
            {
                var table = await ReadTableSchemaAsync(
                    connection,
                    tableKey.Schema,
                    tableKey.Name,
                    cancellationToken);

                tables.Add(table);
            }

            return tables;
        }
        catch (SqlException exception)
        {
            _logger.LogError(exception, "Failed to read SQL Server table schemas.");
            throw;
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "Invalid operation while reading SQL Server table schemas.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TableSchema?> GetTableAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        try
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var tableKey = await ReadTableKeyAsync(connection, tableName, cancellationToken);

            if (tableKey is null)
            {
                return null;
            }

            return await ReadTableSchemaAsync(
                connection,
                tableKey.Schema,
                tableKey.Name,
                cancellationToken);
        }
        catch (SqlException exception)
        {
            _logger.LogError(exception, "Failed to read SQL Server schema for table {TableName}.", tableName);
            throw;
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "Invalid operation while reading SQL Server schema for table {TableName}.", tableName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> TableExistsAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        return await ReadTableKeyAsync(connection, tableName, cancellationToken) is not null;
    }

    private static async Task<List<TableKey>> ReadTableKeysAsync(
        DbConnection connection,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TABLE_SCHEMA, TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = @TableType
            ORDER BY TABLE_SCHEMA, TABLE_NAME;
            """;

        await using var command = CreateCommand(connection, sql);
        AddParameter(command, "@TableType", "BASE TABLE");

        var tables = new List<TableKey>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            tables.Add(new TableKey(
                reader.GetString(0),
                reader.GetString(1)));
        }

        return tables;
    }

    private static async Task<TableKey?> ReadTableKeyAsync(
        DbConnection connection,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (1) TABLE_SCHEMA, TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = @TableType
              AND (TABLE_NAME = @TableName OR CONCAT(TABLE_SCHEMA, '.', TABLE_NAME) = @TableName)
            ORDER BY TABLE_SCHEMA, TABLE_NAME;
            """;

        await using var command = CreateCommand(connection, sql);
        AddParameter(command, "@TableType", "BASE TABLE");
        AddParameter(command, "@TableName", tableName);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new TableKey(reader.GetString(0), reader.GetString(1));
    }

    private static async Task<TableSchema> ReadTableSchemaAsync(
        DbConnection connection,
        string schema,
        string tableName,
        CancellationToken cancellationToken)
    {
        var primaryKey = await ReadPrimaryKeyAsync(connection, schema, tableName, cancellationToken);
        var columns = await ReadColumnsAsync(connection, schema, tableName, primaryKey, cancellationToken);
        var foreignKeys = await ReadForeignKeysAsync(connection, schema, tableName, cancellationToken);
        var indexes = await ReadIndexesAsync(connection, schema, tableName, cancellationToken);

        return new TableSchema
        {
            Name = tableName,
            Schema = schema,
            Columns = columns,
            PrimaryKey = primaryKey,
            ForeignKeys = foreignKeys,
            Indexes = indexes
        };
    }

    private static async Task<List<ColumnSchema>> ReadColumnsAsync(
        DbConnection connection,
        string schema,
        string tableName,
        PrimaryKeySchema? primaryKey,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                c.name AS ColumnName,
                typ.name AS DataType,
                c.is_nullable AS IsNullable,
                CONVERT(bit, CASE WHEN ic.column_id IS NULL THEN 0 ELSE 1 END) AS IsIdentity,
                CASE
                    WHEN typ.name IN ('nchar', 'nvarchar') AND c.max_length > 0 THEN c.max_length / 2
                    ELSE c.max_length
                END AS MaxLength,
                c.precision AS Precision,
                c.scale AS Scale,
                c.is_computed AS IsComputed,
                dc.definition AS DefaultValue,
                c.column_id AS OrdinalPosition
            FROM sys.tables t
            INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
            INNER JOIN sys.columns c ON c.object_id = t.object_id
            INNER JOIN sys.types typ ON typ.user_type_id = c.user_type_id
            LEFT JOIN sys.identity_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            LEFT JOIN sys.default_constraints dc ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
            WHERE s.name = @Schema
              AND t.name = @TableName
            ORDER BY c.column_id;
            """;

        var primaryKeyColumns = primaryKey?.Columns.ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var command = CreateCommand(connection, sql);
        AddParameter(command, "@Schema", schema);
        AddParameter(command, "@TableName", tableName);

        var columns = new List<ColumnSchema>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var name = reader.GetString(0);

            columns.Add(new ColumnSchema
            {
                Name = name,
                DataType = reader.GetString(1),
                IsNullable = reader.GetBoolean(2),
                IsIdentity = reader.GetBoolean(3),
                IsPrimaryKey = primaryKeyColumns.Contains(name),
                MaxLength = GetNullableInt32(reader, 4),
                Precision = GetNullableByteAsInt32(reader, 5),
                Scale = GetNullableInt32(reader, 6),
                IsComputed = reader.GetBoolean(7),
                DefaultValue = await reader.IsDBNullAsync(8, cancellationToken) ? null : reader.GetString(8),
                Order = reader.GetInt32(9)
            });
        }

        return columns;
    }

    private static async Task<PrimaryKeySchema?> ReadPrimaryKeyAsync(
        DbConnection connection,
        string schema,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                kc.name AS ConstraintName,
                c.name AS ColumnName
            FROM sys.tables t
            INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
            INNER JOIN sys.key_constraints kc ON kc.parent_object_id = t.object_id AND kc.type = @ConstraintType
            INNER JOIN sys.index_columns ic ON ic.object_id = t.object_id AND ic.index_id = kc.unique_index_id
            INNER JOIN sys.columns c ON c.object_id = t.object_id AND c.column_id = ic.column_id
            WHERE s.name = @Schema
              AND t.name = @TableName
            ORDER BY ic.key_ordinal;
            """;

        await using var command = CreateCommand(connection, sql);
        AddParameter(command, "@ConstraintType", "PK");
        AddParameter(command, "@Schema", schema);
        AddParameter(command, "@TableName", tableName);

        string? primaryKeyName = null;
        var columns = new List<string>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            primaryKeyName ??= reader.GetString(0);
            columns.Add(reader.GetString(1));
        }

        if (columns.Count == 0)
        {
            return null;
        }

        return new PrimaryKeySchema
        {
            Name = primaryKeyName,
            Columns = columns
        };
    }

    private static async Task<List<ForeignKeySchema>> ReadForeignKeysAsync(
        DbConnection connection,
        string schema,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                fk.name AS ForeignKeyName,
                pc.name AS ColumnName,
                rt.name AS ReferencedTable,
                rc.name AS ReferencedColumn
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
            INNER JOIN sys.tables pt ON pt.object_id = fk.parent_object_id
            INNER JOIN sys.schemas ps ON ps.schema_id = pt.schema_id
            INNER JOIN sys.columns pc ON pc.object_id = pt.object_id AND pc.column_id = fkc.parent_column_id
            INNER JOIN sys.tables rt ON rt.object_id = fk.referenced_object_id
            INNER JOIN sys.columns rc ON rc.object_id = rt.object_id AND rc.column_id = fkc.referenced_column_id
            WHERE ps.name = @Schema
              AND pt.name = @TableName
            ORDER BY fk.name, fkc.constraint_column_id;
            """;

        await using var command = CreateCommand(connection, sql);
        AddParameter(command, "@Schema", schema);
        AddParameter(command, "@TableName", tableName);

        var foreignKeys = new List<ForeignKeySchema>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            foreignKeys.Add(new ForeignKeySchema
            {
                Name = reader.GetString(0),
                Column = reader.GetString(1),
                ReferencedTable = reader.GetString(2),
                ReferencedColumn = reader.GetString(3)
            });
        }

        return foreignKeys;
    }

    private static async Task<List<IndexSchema>> ReadIndexesAsync(
        DbConnection connection,
        string schema,
        string tableName,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                i.name AS IndexName,
                i.is_unique AS IsUnique,
                c.name AS ColumnName
            FROM sys.tables t
            INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
            INNER JOIN sys.indexes i ON i.object_id = t.object_id
            INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
            INNER JOIN sys.columns c ON c.object_id = t.object_id AND c.column_id = ic.column_id
            WHERE s.name = @Schema
              AND t.name = @TableName
              AND i.name IS NOT NULL
              AND i.is_hypothetical = 0
            ORDER BY i.name, ic.key_ordinal, ic.index_column_id;
            """;

        await using var command = CreateCommand(connection, sql);
        AddParameter(command, "@Schema", schema);
        AddParameter(command, "@TableName", tableName);

        var indexes = new Dictionary<string, IndexSchemaBuilder>(StringComparer.OrdinalIgnoreCase);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var indexName = reader.GetString(0);

            if (!indexes.TryGetValue(indexName, out var index))
            {
                index = new IndexSchemaBuilder(indexName, reader.GetBoolean(1));
                indexes.Add(indexName, index);
            }

            index.Columns.Add(reader.GetString(2));
        }

        return indexes.Values.Select(index => new IndexSchema
        {
            Name = index.Name,
            IsUnique = index.IsUnique,
            Columns = index.Columns
        }).ToList();
    }

    private static DbCommand CreateCommand(DbConnection connection, string commandText)
    {
        var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.CommandType = CommandType.Text;
        return command;
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static int? GetNullableInt32(DbDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        var value = reader.GetValue(ordinal);
        return Convert.ToInt32(value);
    }

    private static int? GetNullableByteAsInt32(DbDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        return Convert.ToInt32(reader.GetByte(ordinal));
    }

    private sealed record TableKey(string Schema, string Name);

    private sealed class IndexSchemaBuilder
    {
        public IndexSchemaBuilder(string name, bool isUnique)
        {
            Name = name;
            IsUnique = isUnique;
        }

        public string Name { get; }

        public bool IsUnique { get; }

        public List<string> Columns { get; } = [];
    }
}
