using System.Collections;
using System.Data;
using System.Data.Common;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Services.Schema;
using Microsoft.Extensions.Logging.Abstractions;

#pragma warning disable CS8764, CS8765

namespace MCPTools.Tests;

public sealed class SqlServerSchemaProviderTests
{
    [Fact]
    public async Task TableExistsAsync_ReturnsTrue_WhenTableExists()
    {
        var connection = new FakeDbConnection([
            [["dbo", "Employee"]]
        ]);
        var provider = CreateProvider(connection);

        var exists = await provider.TableExistsAsync("Employee");

        Assert.True(exists);
    }

    [Fact]
    public async Task GetTableAsync_ReturnsPopulatedTableSchema_WhenTableExists()
    {
        var connection = new FakeDbConnection([
            [["dbo", "Employee"]],
            [["PK_Employee", "EmployeeId"]],
            [
                ["EmployeeId", "int", false, true, DBNull.Value, (byte)10, (byte)0, false, DBNull.Value, 1],
                ["FirstName", "nvarchar", false, false, 100, (byte)0, (byte)0, false, DBNull.Value, 2],
                ["DepartmentId", "int", true, false, DBNull.Value, (byte)10, (byte)0, false, "((1))", 3]
            ],
            [["FK_Employee_Department", "DepartmentId", "Department", "DepartmentId"]],
            [["IX_Employee_FirstName", false, "FirstName"]]
        ]);
        var provider = CreateProvider(connection);

        var table = await provider.GetTableAsync("Employee");

        Assert.NotNull(table);
        Assert.Equal("Employee", table.Name);
        Assert.Equal("dbo", table.Schema);
        Assert.Equal(3, table.Columns.Count);
        Assert.Equal("PK_Employee", table.PrimaryKey?.Name);
        Assert.Contains("EmployeeId", table.PrimaryKey?.Columns ?? []);
        Assert.Single(table.ForeignKeys);
        Assert.Single(table.Indexes);
    }

    [Fact]
    public async Task GetTablesAsync_ReturnsAllDiscoveredTables()
    {
        var connection = new FakeDbConnection([
            [["dbo", "Employee"]],
            [["PK_Employee", "EmployeeId"]],
            [["EmployeeId", "int", false, true, DBNull.Value, (byte)10, (byte)0, false, DBNull.Value, 1]],
            [],
            []
        ]);
        var provider = CreateProvider(connection);

        var tables = await provider.GetTablesAsync();

        Assert.Single(tables);
        Assert.Equal("Employee", tables[0].Name);
    }

    private static SqlServerSchemaProvider CreateProvider(FakeDbConnection connection)
    {
        return new SqlServerSchemaProvider(
            new FakeDatabaseConnectionFactory(connection),
            NullLogger<SqlServerSchemaProvider>.Instance);
    }

    private sealed class FakeDatabaseConnectionFactory(FakeDbConnection connection) : IDatabaseConnectionFactory
    {
        public DbConnection CreateConnection()
        {
            return connection;
        }
    }

    private sealed class FakeDbConnection(IReadOnlyList<IReadOnlyList<object?[]>> resultSets) : DbConnection
    {
        private readonly Queue<IReadOnlyList<object?[]>> _resultSets = new(resultSets);
        private ConnectionState _state = ConnectionState.Closed;

        public override string? ConnectionString { get; set; } = string.Empty;

        public override string Database => "TestDatabase";

        public override string DataSource => "TestServer";

        public override string ServerVersion => "1.0";

        public override ConnectionState State => _state;

        public IReadOnlyList<object?[]> DequeueResultSet()
        {
            return _resultSets.Count == 0
                ? []
                : _resultSets.Dequeue();
        }

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
            _state = ConnectionState.Closed;
        }

        public override void Open()
        {
            _state = ConnectionState.Open;
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            _state = ConnectionState.Open;
            return Task.CompletedTask;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new FakeDbCommand(this);
        }
    }

    private sealed class FakeDbCommand(FakeDbConnection connection) : DbCommand
    {
        private readonly FakeDbParameterCollection _parameters = new();

        public override string? CommandText { get; set; } = string.Empty;

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection? DbConnection { get; set; } = connection;

        protected override DbParameterCollection DbParameterCollection => _parameters;

        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            return 0;
        }

        public override object? ExecuteScalar()
        {
            return null;
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new FakeDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return new FakeDbDataReader(connection.DequeueResultSet());
        }

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<DbDataReader>(new FakeDbDataReader(connection.DequeueResultSet()));
        }
    }

    private sealed class FakeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }

        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public override bool IsNullable { get; set; }

        public override string ParameterName { get; set; } = string.Empty;

        public override string? SourceColumn { get; set; } = string.Empty;

        public override object? Value { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override int Size { get; set; }

        public override void ResetDbType()
        {
        }
    }

    private sealed class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<object> _parameters = [];

        public override int Count => _parameters.Count;

        public override object SyncRoot => ((ICollection)_parameters).SyncRoot;

        public override int Add(object value)
        {
            _parameters.Add(value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        public override void Clear()
        {
            _parameters.Clear();
        }

        public override bool Contains(object value)
        {
            return _parameters.Contains(value);
        }

        public override bool Contains(string value)
        {
            return _parameters.OfType<DbParameter>().Any(parameter => parameter.ParameterName == value);
        }

        public override void CopyTo(Array array, int index)
        {
            _parameters.ToArray().CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public override int IndexOf(object value)
        {
            return _parameters.IndexOf(value);
        }

        public override int IndexOf(string parameterName)
        {
            return _parameters
                .OfType<DbParameter>()
                .Select((parameter, index) => new { parameter, index })
                .FirstOrDefault(item => item.parameter.ParameterName == parameterName)
                ?.index ?? -1;
        }

        public override void Insert(int index, object value)
        {
            _parameters.Insert(index, value);
        }

        public override void Remove(object value)
        {
            _parameters.Remove(value);
        }

        public override void RemoveAt(int index)
        {
            _parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);

            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        protected override DbParameter GetParameter(int index)
        {
            return (DbParameter)_parameters[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return (DbParameter)_parameters[IndexOf(parameterName)];
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            _parameters[index] = value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);

            if (index >= 0)
            {
                _parameters[index] = value;
            }
        }
    }

    private sealed class FakeDbDataReader(IReadOnlyList<object?[]> rows) : DbDataReader
    {
        private int _index = -1;

        public override int Depth => 0;

        public override int FieldCount => rows.Count == 0 ? 0 : rows[0].Length;

        public override bool HasRows => rows.Count > 0;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => throw new NotSupportedException();

        public override bool GetBoolean(int ordinal)
        {
            return Convert.ToBoolean(GetValue(ordinal));
        }

        public override byte GetByte(int ordinal)
        {
            return Convert.ToByte(GetValue(ordinal));
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            return 0;
        }

        public override char GetChar(int ordinal)
        {
            return Convert.ToChar(GetValue(ordinal));
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            return 0;
        }

        public override string GetDataTypeName(int ordinal)
        {
            return GetFieldType(ordinal).Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return Convert.ToDateTime(GetValue(ordinal));
        }

        public override decimal GetDecimal(int ordinal)
        {
            return Convert.ToDecimal(GetValue(ordinal));
        }

        public override double GetDouble(int ordinal)
        {
            return Convert.ToDouble(GetValue(ordinal));
        }

        public override IEnumerator GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            var value = GetValue(ordinal);
            return value == DBNull.Value ? typeof(DBNull) : value.GetType();
        }

        public override float GetFloat(int ordinal)
        {
            return Convert.ToSingle(GetValue(ordinal));
        }

        public override Guid GetGuid(int ordinal)
        {
            return (Guid)GetValue(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return Convert.ToInt16(GetValue(ordinal));
        }

        public override int GetInt32(int ordinal)
        {
            return Convert.ToInt32(GetValue(ordinal));
        }

        public override long GetInt64(int ordinal)
        {
            return Convert.ToInt64(GetValue(ordinal));
        }

        public override string GetName(int ordinal)
        {
            return string.Empty;
        }

        public override int GetOrdinal(string name)
        {
            throw new NotSupportedException();
        }

        public override string GetString(int ordinal)
        {
            return Convert.ToString(GetValue(ordinal)) ?? string.Empty;
        }

        public override object GetValue(int ordinal)
        {
            var value = rows[_index][ordinal];
            return value ?? DBNull.Value;
        }

        public override int GetValues(object[] values)
        {
            var count = Math.Min(values.Length, FieldCount);

            for (var index = 0; index < count; index++)
            {
                values[index] = GetValue(index);
            }

            return count;
        }

        public override bool IsDBNull(int ordinal)
        {
            return GetValue(ordinal) == DBNull.Value;
        }

        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
        {
            return Task.FromResult(IsDBNull(ordinal));
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            _index++;
            return _index < rows.Count;
        }

        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Read());
        }
    }
}
