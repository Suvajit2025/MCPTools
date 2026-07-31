using System.Globalization;
using System.Text;
using MCPTools.Core.Constants;
using MCPTools.Core.Models.Generation;

namespace MCPTools.Core.Services;

/// <summary>
/// Builds placeholder dictionaries used by the template engine.
/// </summary>
public sealed class PlaceholderBuilder
{
    private const string DefaultVersion = "1.0.0";
    private const string DefaultApiVersion = "1";

    /// <summary>
    /// Builds a placeholder dictionary for the specified entity definition.
    /// </summary>
    /// <param name="entity">The entity definition used to build placeholders.</param>
    /// <returns>A read-only dictionary containing placeholder values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <see langword="null"/>.</exception>
    public IReadOnlyDictionary<string, string> Build(EntityDefinition entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var values = new Dictionary<string, string>(StringComparer.Ordinal);

        BuildProjectPlaceholders(values, entity);
        BuildEntityPlaceholders(values, entity);
        BuildNamePlaceholders(values, entity);
        BuildFilePlaceholders(values, entity);
        BuildNamespacePlaceholders(values, entity);
        BuildClassPlaceholders(values, entity);
        BuildPropertyPlaceholders(values, entity);
        BuildRequestResponsePlaceholders(values, entity);
        BuildSqlPlaceholders(values, entity);
        BuildApiPlaceholders(values, entity);
        BuildAngularPlaceholders(values, entity);
        BuildReactPlaceholders(values, entity);
        BuildMiscellaneousPlaceholders(values, entity);

        return values;
    }

    private static void BuildProjectPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.SolutionName, GetRootNamespace(entity.Namespace));
        Set(values, PlaceholderConstants.ProjectName, entity.Namespace);
        Set(values, PlaceholderConstants.ProjectNamespace, entity.Namespace);
        Set(values, PlaceholderConstants.RootNamespace, entity.Namespace);
        Set(values, PlaceholderConstants.CompanyName, entity.CompanyName);
        Set(values, PlaceholderConstants.Author, entity.Author);
        Set(values, PlaceholderConstants.Date, DateTimeOffset.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        Set(values, PlaceholderConstants.Year, DateTimeOffset.UtcNow.Year.ToString(CultureInfo.InvariantCulture));
        Set(values, PlaceholderConstants.Version, DefaultVersion);
    }

    private static void BuildEntityPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        var primaryKeyProperty = FindPrimaryKeyProperty(entity);

        Set(values, PlaceholderConstants.EntityName, entity.EntityName);
        Set(values, PlaceholderConstants.PluralEntityName, entity.PluralEntityName);
        Set(values, PlaceholderConstants.EntityDisplayName, SplitPascalCase(entity.EntityName));
        Set(values, PlaceholderConstants.TableName, entity.TableName);
        Set(values, PlaceholderConstants.SchemaName, entity.Schema);
        Set(values, PlaceholderConstants.PrimaryKey, entity.PrimaryKey);
        Set(values, PlaceholderConstants.PrimaryKeyType, entity.PrimaryKeyType);
        Set(values, PlaceholderConstants.PrimaryKeyColumn, primaryKeyProperty is null ? entity.PrimaryKey : GetColumnName(primaryKeyProperty));
    }

    private static void BuildNamePlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        var entityName = entity.EntityName;

        Set(values, PlaceholderConstants.RepositoryName, $"{entityName}Repository");
        Set(values, PlaceholderConstants.ServiceName, $"{entityName}Service");
        Set(values, PlaceholderConstants.ControllerName, $"{entityName}Controller");
        Set(values, PlaceholderConstants.ManagerName, $"{entityName}Manager");
        Set(values, PlaceholderConstants.DtoName, $"{entityName}Dto");
        Set(values, PlaceholderConstants.MapperName, $"{entityName}Mapper");
        Set(values, PlaceholderConstants.ValidatorName, $"{entityName}Validator");
        Set(values, PlaceholderConstants.InterfaceName, $"I{entityName}");
    }

    private static void BuildFilePlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.FileName, $"{entity.EntityName}.cs");
        Set(values, PlaceholderConstants.FileExtension, FileExtensions.CSharp);
        Set(values, PlaceholderConstants.FolderName, entity.EntityName);
        Set(values, PlaceholderConstants.RelativePath, entity.EntityName);
    }

    private static void BuildNamespacePlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.Namespace, entity.Namespace);
        Set(values, PlaceholderConstants.ApplicationNamespace, $"{entity.Namespace}.Application");
        Set(values, PlaceholderConstants.DomainNamespace, $"{entity.Namespace}.Domain");
        Set(values, PlaceholderConstants.InfrastructureNamespace, $"{entity.Namespace}.Infrastructure");
        Set(values, PlaceholderConstants.ApiNamespace, $"{entity.Namespace}.Api");
        Set(values, PlaceholderConstants.SharedNamespace, $"{entity.Namespace}.Shared");
    }

    private static void BuildClassPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.ClassName, entity.EntityName);
        Set(values, PlaceholderConstants.BaseClass, entity.BaseClass);
        Set(values, PlaceholderConstants.Interfaces, string.Join(", ", entity.Interfaces));
        Set(values, PlaceholderConstants.ClassAttributes, string.Empty);
        Set(values, PlaceholderConstants.Constructor, string.Empty);
        Set(values, PlaceholderConstants.Properties, BuildPropertyDeclarations(entity.Properties));
        Set(values, PlaceholderConstants.Methods, string.Empty);
        Set(values, PlaceholderConstants.AdditionalMethods, string.Empty);
    }

    private static void BuildPropertyPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        var firstProperty = GetOrderedProperties(entity.Properties).FirstOrDefault();

        Set(values, PlaceholderConstants.PropertyName, firstProperty?.Name);
        Set(values, PlaceholderConstants.PropertyType, firstProperty is null ? null : BuildTypeName(firstProperty));
        Set(values, PlaceholderConstants.PropertyDeclarations, BuildPropertyDeclarations(entity.Properties));
        Set(values, PlaceholderConstants.PropertyAssignments, BuildPropertyAssignments(entity.Properties));
        Set(values, PlaceholderConstants.NavigationProperties, string.Empty);
    }

    private static void BuildRequestResponsePlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.CreateRequest, $"{entity.EntityName}CreateRequest");
        Set(values, PlaceholderConstants.UpdateRequest, $"{entity.EntityName}UpdateRequest");
        Set(values, PlaceholderConstants.Response, $"{entity.EntityName}Response");
        Set(values, PlaceholderConstants.PagedResponse, $"PagedResult<{entity.EntityName}Response>");
    }

    private static void BuildSqlPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.StoredProcedureName, entity.EntityName);
        Set(values, PlaceholderConstants.Columns, BuildSelectColumns(entity.Properties));
        Set(values, PlaceholderConstants.ParameterList, BuildParameterList(entity.Properties));
        Set(values, PlaceholderConstants.SqlParameters, BuildSqlParameters(entity.Properties));
        Set(values, PlaceholderConstants.InsertSqlParameters, BuildInsertSqlParameters(entity.Properties));
        Set(values, PlaceholderConstants.UpdateSqlParameters, BuildUpdateSqlParameters(entity));
        Set(values, PlaceholderConstants.PrimaryKeySqlParameters, BuildPrimaryKeySqlParameters(entity));
        Set(values, PlaceholderConstants.InsertColumns, BuildInsertColumns(entity.Properties));
        Set(values, PlaceholderConstants.InsertValues, BuildInsertValues(entity.Properties));
        Set(values, PlaceholderConstants.UpdateSetClause, BuildUpdateSetClause(entity));
        Set(values, PlaceholderConstants.PrimaryKeyWhere, BuildPrimaryKeyWhere(entity));
        Set(values, PlaceholderConstants.SelectColumns, BuildSelectColumns(entity.Properties));
        Set(values, PlaceholderConstants.ConnectionString, string.Empty);
        Set(values, PlaceholderConstants.Database, entity.TableName);
    }

    private static void BuildApiPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.Route, entity.PluralEntityName.ToLowerInvariant());
        Set(values, PlaceholderConstants.HttpMethod, "GET");
        Set(values, PlaceholderConstants.Endpoint, $"/api/v{DefaultApiVersion}/{entity.PluralEntityName.ToLowerInvariant()}");
        Set(values, PlaceholderConstants.ApiVersion, DefaultApiVersion);
    }

    private static void BuildAngularPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        var kebabName = ToKebabCase(entity.EntityName);

        Set(values, PlaceholderConstants.ComponentName, $"{entity.EntityName}Component");
        Set(values, PlaceholderConstants.Selector, $"app-{kebabName}");
        Set(values, PlaceholderConstants.ModuleName, $"{entity.EntityName}Module");
        Set(values, PlaceholderConstants.ServiceFile, $"{kebabName}.service.ts");
        Set(values, PlaceholderConstants.HtmlTemplate, $"{kebabName}.component.html");
    }

    private static void BuildReactPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        Set(values, PlaceholderConstants.Component, entity.EntityName);
        Set(values, PlaceholderConstants.Props, $"{entity.EntityName}Props");
        Set(values, PlaceholderConstants.Hooks, $"use{entity.EntityName}");
    }

    private static void BuildMiscellaneousPlaceholders(
        IDictionary<string, string> values,
        EntityDefinition entity)
    {
        var timestamp = DateTimeOffset.UtcNow;

        Set(values, PlaceholderConstants.Guid, System.Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture));
        Set(values, PlaceholderConstants.Timestamp, timestamp.ToString("O", CultureInfo.InvariantCulture));
        Set(values, PlaceholderConstants.GeneratedWarning, "This file was generated by MCPTools. Do not edit manually.");
        Set(values, PlaceholderConstants.Copyright, BuildCopyright(entity, timestamp));
    }

    private static string BuildPropertyDeclarations(IEnumerable<PropertyDefinition> properties)
    {
        return string.Join(
            Environment.NewLine,
            GetOrderedProperties(properties)
                .Select(property => $"    public {BuildTypeName(property)} {property.Name} {{ get; init; }}"));
    }

    private static string BuildPropertyAssignments(IEnumerable<PropertyDefinition> properties)
    {
        return JoinCodeLines(
            GetOrderedProperties(properties)
                .Select(property => $"            {property.Name} = source.{property.Name}"));
    }

    private static string BuildParameterList(IEnumerable<PropertyDefinition> properties)
    {
        return string.Join(
            Environment.NewLine,
            GetInsertableProperties(properties)
                .Select(property => $"        parameters.Add(\"@{GetColumnName(property)}\", entity.{property.Name});"));
    }

    private static string BuildSqlParameters(IEnumerable<PropertyDefinition> properties)
    {
        return JoinSqlLines(
            GetOrderedProperties(properties)
                .Select(BuildSqlParameter));
    }

    private static string BuildInsertSqlParameters(IEnumerable<PropertyDefinition> properties)
    {
        return JoinSqlLines(
            GetInsertableProperties(properties)
                .Select(BuildSqlParameter));
    }

    private static string BuildUpdateSqlParameters(EntityDefinition entity)
    {
        var primaryKeyProperty = FindPrimaryKeyProperty(entity);
        var parameterLines = primaryKeyProperty is null
            ? [BuildSqlParameter(entity.PrimaryKey, entity.PrimaryKeyType)]
            : new[] { BuildSqlParameter(primaryKeyProperty) };

        return JoinSqlLines(
            parameterLines.Concat(GetUpdatableProperties(entity).Select(BuildSqlParameter)));
    }

    private static string BuildPrimaryKeySqlParameters(EntityDefinition entity)
    {
        var primaryKeyProperty = FindPrimaryKeyProperty(entity);

        return primaryKeyProperty is null
            ? BuildSqlParameter(entity.PrimaryKey, entity.PrimaryKeyType)
            : BuildSqlParameter(primaryKeyProperty);
    }

    private static string BuildInsertColumns(IEnumerable<PropertyDefinition> properties)
    {
        return JoinSqlLines(
            GetInsertableProperties(properties)
                .Select(property => $"        [{GetColumnName(property)}]"));
    }

    private static string BuildInsertValues(IEnumerable<PropertyDefinition> properties)
    {
        return JoinSqlLines(
            GetInsertableProperties(properties)
                .Select(property => $"        @{GetColumnName(property)}"));
    }

    private static string BuildUpdateSetClause(EntityDefinition entity)
    {
        return JoinSqlLines(
            GetUpdatableProperties(entity)
                .Select(property => $"        [{GetColumnName(property)}] = @{GetColumnName(property)}"));
    }

    private static string BuildPrimaryKeyWhere(EntityDefinition entity)
    {
        var primaryKeyProperty = FindPrimaryKeyProperty(entity);
        var primaryKeyColumn = primaryKeyProperty is null
            ? entity.PrimaryKey
            : GetColumnName(primaryKeyProperty);

        return $"        [{primaryKeyColumn}] = @{primaryKeyColumn}";
    }

    private static string BuildSelectColumns(IEnumerable<PropertyDefinition> properties)
    {
        return JoinSqlLines(
            GetOrderedProperties(properties)
                .Select(property => $"        [{GetColumnName(property)}]"));
    }

    private static string BuildTypeName(PropertyDefinition property)
    {
        if (!property.IsNullable || property.Type.EndsWith("?", StringComparison.Ordinal))
        {
            return property.Type;
        }

        return $"{property.Type}?";
    }

    private static string BuildSqlTypeName(PropertyDefinition property)
    {
        if (!string.IsNullOrWhiteSpace(property.SqlType))
        {
            return BuildSqlTypeName(property.SqlType, property.MaxLength, property.Precision, property.Scale);
        }

        return BuildSqlTypeName(property.Type, property.MaxLength, property.Precision, property.Scale);
    }

    private static string BuildSqlTypeName(
        string typeName,
        int? maxLength = null,
        int? precision = null,
        int? scale = null)
    {
        return GetNonNullableTypeName(typeName).ToLowerInvariant() switch
        {
            "byte" => "TINYINT",
            "short" or "int16" => "SMALLINT",
            "int" or "int32" => "INT",
            "long" or "int64" => "BIGINT",
            "bool" or "boolean" => "BIT",
            "decimal" => $"DECIMAL({precision.GetValueOrDefault(18)}, {scale.GetValueOrDefault(2)})",
            "double" => "FLOAT",
            "float" or "single" => "REAL",
            "datetime" => "DATETIME2",
            "datetimeoffset" => "DATETIMEOFFSET",
            "dateonly" => "DATE",
            "date" => "DATE",
            "nvarchar" => BuildSqlStringType("NVARCHAR", maxLength),
            "varchar" => BuildSqlStringType("VARCHAR", maxLength),
            "nchar" => BuildSqlStringType("NCHAR", maxLength),
            "char" => BuildSqlStringType("CHAR", maxLength),
            "numeric" => $"NUMERIC({precision.GetValueOrDefault(18)},{scale.GetValueOrDefault(2)})",
            "money" => "MONEY",
            "smallmoney" => "SMALLMONEY",
            "timeonly" or "timespan" => "TIME",
            "time" => "TIME",
            "guid" => "UNIQUEIDENTIFIER",
            "uniqueidentifier" => "UNIQUEIDENTIFIER",
            "byte[]" => BuildSqlBinaryType("VARBINARY", maxLength),
            "binary" => BuildSqlBinaryType("BINARY", maxLength),
            "varbinary" => BuildSqlBinaryType("VARBINARY", maxLength),
            "string" => BuildSqlStringType("NVARCHAR", maxLength),
            "xml" => "XML",
            _ => "NVARCHAR(MAX)"
        };
    }

    private static string BuildSqlStringType(
        string sqlType,
        int? maxLength)
    {
        return maxLength is > 0
            ? $"{sqlType}({maxLength.Value})"
            : $"{sqlType}(MAX)";
    }

    private static string BuildSqlBinaryType(
        string sqlType,
        int? maxLength)
    {
        return maxLength is > 0
            ? $"{sqlType}({maxLength.Value})"
            : $"{sqlType}(MAX)";
    }

    private static string BuildSqlParameter(PropertyDefinition property)
    {
        return $"    @{GetColumnName(property)} {BuildSqlTypeName(property)}";
    }

    private static string BuildSqlParameter(string name, string typeName)
    {
        return $"    @{name} {BuildSqlTypeName(typeName)}";
    }

    private static string BuildCopyright(
        EntityDefinition entity,
        DateTimeOffset timestamp)
    {
        return string.IsNullOrWhiteSpace(entity.CompanyName)
            ? $"Copyright (c) {timestamp.Year}"
            : $"Copyright (c) {timestamp.Year} {entity.CompanyName}";
    }

    private static IEnumerable<PropertyDefinition> GetInsertableProperties(IEnumerable<PropertyDefinition> properties)
    {
        return GetOrderedProperties(properties)
            .Where(property => !property.IsIdentity && !property.IsComputed);
    }

    private static IEnumerable<PropertyDefinition> GetUpdatableProperties(EntityDefinition entity)
    {
        return GetOrderedProperties(entity.Properties)
            .Where(property => !property.IsIdentity && !property.IsComputed && !IsPrimaryKeyProperty(property, entity));
    }

    private static IEnumerable<PropertyDefinition> GetOrderedProperties(IEnumerable<PropertyDefinition> properties)
    {
        return properties.OrderBy(property => property.Order);
    }

    private static PropertyDefinition? FindPrimaryKeyProperty(EntityDefinition entity)
    {
        return GetOrderedProperties(entity.Properties)
            .FirstOrDefault(property => IsPrimaryKeyProperty(property, entity));
    }

    private static bool IsPrimaryKeyProperty(PropertyDefinition property, EntityDefinition entity)
    {
        return property.IsPrimaryKey ||
            string.Equals(property.Name, entity.PrimaryKey, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(GetColumnName(property), entity.PrimaryKey, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetColumnName(PropertyDefinition property)
    {
        return string.IsNullOrWhiteSpace(property.ColumnName)
            ? property.Name
            : property.ColumnName;
    }

    private static string GetNonNullableTypeName(string typeName)
    {
        return typeName.EndsWith("?", StringComparison.Ordinal)
            ? typeName[..^1]
            : typeName;
    }

    private static string GetRootNamespace(string namespaceName)
    {
        var separatorIndex = namespaceName.IndexOf('.', StringComparison.Ordinal);
        return separatorIndex < 0 ? namespaceName : namespaceName[..separatorIndex];
    }

    private static string SplitPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length + 8);
        builder.Append(value[0]);

        for (var index = 1; index < value.Length; index++)
        {
            if (char.IsUpper(value[index]) && !char.IsWhiteSpace(value[index - 1]))
            {
                builder.Append(' ');
            }

            builder.Append(value[index]);
        }

        return builder.ToString();
    }

    private static string ToKebabCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length + 8);

        for (var index = 0; index < value.Length; index++)
        {
            var current = value[index];

            if (char.IsUpper(current) && index > 0)
            {
                builder.Append('-');
            }

            builder.Append(char.ToLowerInvariant(current));
        }

        return builder.ToString();
    }

    private static string JoinSqlLines(IEnumerable<string> lines)
    {
        return string.Join($",{Environment.NewLine}", lines);
    }

    private static string JoinCodeLines(IEnumerable<string> lines)
    {
        return string.Join($",{Environment.NewLine}", lines);
    }

    private static void Set(
        IDictionary<string, string> values,
        string placeholder,
        string? value)
    {
        values[placeholder] = value ?? string.Empty;
    }
}
