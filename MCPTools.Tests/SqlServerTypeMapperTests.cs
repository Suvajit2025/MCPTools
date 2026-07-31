using MCPTools.Core.Exceptions;
using MCPTools.Core.Models.Schema;
using MCPTools.Core.Services.Schema;

namespace MCPTools.Tests;

public sealed class SqlServerTypeMapperTests
{
    [Theory]
    [InlineData("int", "int")]
    [InlineData("bigint", "long")]
    [InlineData("nvarchar", "string")]
    [InlineData("varchar", "string")]
    [InlineData("decimal", "decimal")]
    [InlineData("bit", "bool")]
    [InlineData("datetime", "DateTime")]
    [InlineData("datetime2", "DateTime")]
    [InlineData("date", "DateOnly")]
    [InlineData("time", "TimeOnly")]
    [InlineData("uniqueidentifier", "Guid")]
    public void MapToClrType_ReturnsExpectedClrType_WhenSqlTypeIsSupported(
        string sqlType,
        string expectedClrType)
    {
        var mapper = new SqlServerTypeMapper();

        var clrType = mapper.MapToClrType(new ColumnSchema
        {
            Name = "Value",
            DataType = sqlType
        });

        Assert.Equal(expectedClrType, clrType);
    }

    [Fact]
    public void MapToClrType_ReturnsNullableValueType_WhenColumnIsNullable()
    {
        var mapper = new SqlServerTypeMapper();

        var clrType = mapper.MapToClrType(new ColumnSchema
        {
            Name = "Salary",
            DataType = "decimal",
            IsNullable = true
        });

        Assert.Equal("decimal?", clrType);
    }

    [Fact]
    public void BuildSqlDeclaration_ReturnsLengthAndPrecision_WhenMetadataIsAvailable()
    {
        var mapper = new SqlServerTypeMapper();

        var textType = mapper.BuildSqlDeclaration(new ColumnSchema
        {
            Name = "FirstName",
            DataType = "nvarchar",
            MaxLength = 100
        });
        var decimalType = mapper.BuildSqlDeclaration(new ColumnSchema
        {
            Name = "Salary",
            DataType = "decimal",
            Precision = 18,
            Scale = 2
        });

        Assert.Equal("NVARCHAR(100)", textType);
        Assert.Equal("DECIMAL(18,2)", decimalType);
    }

    [Fact]
    public void MapToClrType_ThrowsValidationException_WhenSqlTypeIsUnsupported()
    {
        var mapper = new SqlServerTypeMapper();

        Assert.Throws<ToolValidationException>(() => mapper.MapToClrType(new ColumnSchema
        {
            Name = "Value",
            DataType = "geography"
        }));
    }
}
