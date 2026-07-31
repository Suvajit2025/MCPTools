using MCPTools.Core.Exceptions;
using MCPTools.Core.Models.Schema;

namespace MCPTools.Core.Services.Schema;

/// <summary>
/// Maps SQL Server data types to CLR and SQL declaration types used by generation.
/// </summary>
public sealed class SqlServerTypeMapper
{
    /// <summary>
    /// Maps a SQL Server column to a CLR type name.
    /// </summary>
    /// <param name="column">The SQL Server column metadata.</param>
    /// <returns>The CLR type name.</returns>
    /// <exception cref="ToolValidationException">Thrown when the SQL type is unsupported.</exception>
    public string MapToClrType(ColumnSchema column)
    {
        ArgumentNullException.ThrowIfNull(column);

        var clrType = GetClrType(column.DataType);

        if (!column.IsNullable || clrType is "string" or "byte[]")
        {
            return clrType;
        }

        return $"{clrType}?";
    }

    /// <summary>
    /// Builds a SQL parameter type declaration for a SQL Server column.
    /// </summary>
    /// <param name="column">The SQL Server column metadata.</param>
    /// <returns>The SQL parameter type declaration.</returns>
    /// <exception cref="ToolValidationException">Thrown when the SQL type is unsupported.</exception>
    public string BuildSqlDeclaration(ColumnSchema column)
    {
        ArgumentNullException.ThrowIfNull(column);

        return BuildSqlDeclaration(
            column.DataType,
            column.MaxLength,
            column.Precision,
            column.Scale);
    }

    /// <summary>
    /// Builds a SQL parameter type declaration.
    /// </summary>
    /// <param name="sqlType">The SQL Server data type.</param>
    /// <param name="maxLength">The optional maximum length.</param>
    /// <param name="precision">The optional numeric precision.</param>
    /// <param name="scale">The optional numeric scale.</param>
    /// <returns>The SQL parameter type declaration.</returns>
    /// <exception cref="ToolValidationException">Thrown when the SQL type is unsupported.</exception>
    public string BuildSqlDeclaration(
        string? sqlType,
        int? maxLength = null,
        int? precision = null,
        int? scale = null)
    {
        return NormalizeType(sqlType) switch
        {
            "bigint" => "BIGINT",
            "binary" => BuildLengthDeclaration("BINARY", maxLength),
            "bit" => "BIT",
            "char" => BuildLengthDeclaration("CHAR", maxLength),
            "date" => "DATE",
            "datetime" => "DATETIME",
            "datetime2" => "DATETIME2",
            "datetimeoffset" => "DATETIMEOFFSET",
            "decimal" => $"DECIMAL({precision.GetValueOrDefault(18)},{scale.GetValueOrDefault(2)})",
            "float" => "FLOAT",
            "int" => "INT",
            "money" => "MONEY",
            "nchar" => BuildLengthDeclaration("NCHAR", maxLength),
            "numeric" => $"NUMERIC({precision.GetValueOrDefault(18)},{scale.GetValueOrDefault(2)})",
            "nvarchar" => BuildLengthDeclaration("NVARCHAR", maxLength),
            "real" => "REAL",
            "smalldatetime" => "SMALLDATETIME",
            "smallint" => "SMALLINT",
            "smallmoney" => "SMALLMONEY",
            "time" => "TIME",
            "tinyint" => "TINYINT",
            "uniqueidentifier" => "UNIQUEIDENTIFIER",
            "varbinary" => BuildLengthDeclaration("VARBINARY", maxLength),
            "varchar" => BuildLengthDeclaration("VARCHAR", maxLength),
            "xml" => "XML",
            _ => throw new ToolValidationException($"Unsupported SQL Server data type '{sqlType}'.")
        };
    }

    private static string GetClrType(string? sqlType)
    {
        return NormalizeType(sqlType) switch
        {
            "bigint" => "long",
            "binary" => "byte[]",
            "bit" => "bool",
            "char" => "string",
            "date" => "DateOnly",
            "datetime" => "DateTime",
            "datetime2" => "DateTime",
            "datetimeoffset" => "DateTimeOffset",
            "decimal" => "decimal",
            "float" => "double",
            "int" => "int",
            "money" => "decimal",
            "nchar" => "string",
            "numeric" => "decimal",
            "nvarchar" => "string",
            "real" => "float",
            "smalldatetime" => "DateTime",
            "smallint" => "short",
            "smallmoney" => "decimal",
            "time" => "TimeOnly",
            "tinyint" => "byte",
            "uniqueidentifier" => "Guid",
            "varbinary" => "byte[]",
            "varchar" => "string",
            "xml" => "string",
            _ => throw new ToolValidationException($"Unsupported SQL Server data type '{sqlType}'.")
        };
    }

    private static string BuildLengthDeclaration(
        string sqlType,
        int? maxLength)
    {
        return maxLength is null or < 0
            ? $"{sqlType}(MAX)"
            : $"{sqlType}({maxLength.Value})";
    }

    private static string NormalizeType(string? sqlType)
    {
        if (string.IsNullOrWhiteSpace(sqlType))
        {
            throw new ToolValidationException("SQL Server data type is required.");
        }

        return sqlType.Trim().ToLowerInvariant();
    }
}
