using MCPTools.Core.Constants;
using MCPTools.Core.Models.Generation;
using MCPTools.Core.Services;
using System.Reflection;

namespace MCPTools.Tests;

public sealed class PlaceholderBuilderTests
{
    [Fact]
    public void PlaceholderConstants_ContainUniquePlaceholderValues()
    {
        var placeholders = GetCatalogPlaceholders();

        Assert.Equal(placeholders.Count, placeholders.Distinct(StringComparer.Ordinal).Count());
        Assert.All(placeholders, placeholder =>
        {
            Assert.DoesNotContain("{{", placeholder, StringComparison.Ordinal);
            Assert.DoesNotContain("}}", placeholder, StringComparison.Ordinal);
            Assert.False(string.IsNullOrWhiteSpace(placeholder));
        });
    }

    [Fact]
    public void Build_CreatesValueForEveryCatalogPlaceholder()
    {
        var builder = new PlaceholderBuilder();
        var entity = CreateEmployeeEntity();

        var placeholders = builder.Build(entity);

        Assert.All(GetCatalogPlaceholders(), placeholder => Assert.True(placeholders.ContainsKey(placeholder), placeholder));
    }

    [Fact]
    public void Build_CreatesDedicatedSqlPlaceholders_WhenEntityHasDatabaseMetadata()
    {
        var builder = new PlaceholderBuilder();
        var entity = CreateEmployeeEntity();

        var placeholders = builder.Build(entity);

        Assert.Contains("@EmployeeId INT", placeholders[PlaceholderConstants.SqlParameters], StringComparison.Ordinal);
        Assert.Contains("@FirstName NVARCHAR(100)", placeholders[PlaceholderConstants.SqlParameters], StringComparison.Ordinal);
        Assert.Contains("@Salary DECIMAL(18, 2)", placeholders[PlaceholderConstants.SqlParameters], StringComparison.Ordinal);
        Assert.DoesNotContain("@EmployeeId", placeholders[PlaceholderConstants.InsertSqlParameters], StringComparison.Ordinal);
        Assert.Contains("@EmployeeId INT", placeholders[PlaceholderConstants.UpdateSqlParameters], StringComparison.Ordinal);
        Assert.Equal("    @EmployeeId INT", placeholders[PlaceholderConstants.PrimaryKeySqlParameters]);
        Assert.Contains("[EmployeeId]", placeholders[PlaceholderConstants.SelectColumns], StringComparison.Ordinal);
        Assert.Contains("[FirstName]", placeholders[PlaceholderConstants.InsertColumns], StringComparison.Ordinal);
        Assert.Contains("@FirstName", placeholders[PlaceholderConstants.InsertValues], StringComparison.Ordinal);
        Assert.Contains("[FirstName] = @FirstName", placeholders[PlaceholderConstants.UpdateSetClause], StringComparison.Ordinal);
        Assert.Equal("        [EmployeeId] = @EmployeeId", placeholders[PlaceholderConstants.PrimaryKeyWhere]);
    }

    [Fact]
    public void Build_ExcludesIdentityColumns_FromInsertAndUpdateFragments()
    {
        var builder = new PlaceholderBuilder();
        var entity = CreateEmployeeEntity();

        var placeholders = builder.Build(entity);

        Assert.DoesNotContain("[EmployeeId]", placeholders[PlaceholderConstants.InsertColumns], StringComparison.Ordinal);
        Assert.DoesNotContain("@EmployeeId", placeholders[PlaceholderConstants.InsertValues], StringComparison.Ordinal);
        Assert.DoesNotContain("[EmployeeId] = @EmployeeId", placeholders[PlaceholderConstants.UpdateSetClause], StringComparison.Ordinal);
    }

    [Fact]
    public void Build_PreservesColumnOrdering_InSqlFragments()
    {
        var builder = new PlaceholderBuilder();
        var entity = CreateEmployeeEntity();

        var placeholders = builder.Build(entity);
        var selectColumns = placeholders[PlaceholderConstants.SelectColumns];

        Assert.True(
            selectColumns.IndexOf("[EmployeeId]", StringComparison.Ordinal) <
            selectColumns.IndexOf("[FirstName]", StringComparison.Ordinal));
        Assert.True(
            selectColumns.IndexOf("[FirstName]", StringComparison.Ordinal) <
            selectColumns.IndexOf("[Salary]", StringComparison.Ordinal));
    }

    [Fact]
    public void Build_ExcludesPrimaryKeyFromUpdateSetClause_WhenPrimaryKeyFlagIsNotSet()
    {
        var builder = new PlaceholderBuilder();
        var entity = CreateEmployeeEntityWithUnflaggedPrimaryKey();

        var placeholders = builder.Build(entity);

        Assert.DoesNotContain("[EmployeeId] = @EmployeeId", placeholders[PlaceholderConstants.UpdateSetClause], StringComparison.Ordinal);
        Assert.Equal("        [EmployeeId] = @EmployeeId", placeholders[PlaceholderConstants.PrimaryKeyWhere]);
    }

    private static EntityDefinition CreateEmployeeEntity()
    {
        return new EntityDefinition
        {
            Namespace = "Demo.HRMS",
            EntityName = "Employee",
            PluralEntityName = "Employees",
            TableName = "Employees",
            PrimaryKey = "EmployeeId",
            PrimaryKeyType = "int",
            Properties =
            [
                new PropertyDefinition
                {
                    Name = "EmployeeId",
                    Type = "int",
                    ColumnName = "EmployeeId",
                    IsPrimaryKey = true,
                    IsIdentity = true,
                    Order = 0
                },
                new PropertyDefinition
                {
                    Name = "FirstName",
                    Type = "string",
                    ColumnName = "FirstName",
                    MaxLength = 100,
                    Order = 1
                },
                new PropertyDefinition
                {
                    Name = "Salary",
                    Type = "decimal",
                    ColumnName = "Salary",
                    Precision = 18,
                    Scale = 2,
                    Order = 2
                }
            ]
        };
    }

    private static EntityDefinition CreateEmployeeEntityWithUnflaggedPrimaryKey()
    {
        return new EntityDefinition
        {
            Namespace = "Demo.HRMS",
            EntityName = "Employee",
            PluralEntityName = "Employees",
            TableName = "Employees",
            PrimaryKey = "EmployeeId",
            PrimaryKeyType = "int",
            Properties =
            [
                new PropertyDefinition
                {
                    Name = "EmployeeId",
                    Type = "int",
                    ColumnName = "EmployeeId",
                    Order = 0
                },
                new PropertyDefinition
                {
                    Name = "FirstName",
                    Type = "string",
                    ColumnName = "FirstName",
                    MaxLength = 100,
                    Order = 1
                }
            ]
        };
    }

    private static IReadOnlyList<string> GetCatalogPlaceholders()
    {
        return typeof(PlaceholderConstants)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!)
            .ToArray();
    }
}
