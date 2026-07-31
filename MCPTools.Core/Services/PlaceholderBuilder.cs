using MCPTools.Core.Constants;
using MCPTools.Core.Models.Generation;

namespace MCPTools.Core.Services;

/// <summary>
/// Builds placeholder dictionaries used by the template engine.
/// </summary>
public sealed class PlaceholderBuilder
{
    /// <summary>
    /// Builds a placeholder dictionary for the specified entity definition.
    /// </summary>
    /// <param name="entity">The entity definition used to build placeholders.</param>
    /// <returns>A read-only dictionary containing placeholder values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <see langword="null"/>.</exception>
    public IReadOnlyDictionary<string, string> Build(EntityDefinition entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [PlaceholderConstants.Namespace] = entity.Namespace,
            [PlaceholderConstants.EntityName] = entity.EntityName,
            [PlaceholderConstants.PluralEntityName] = entity.PluralEntityName,
            [PlaceholderConstants.TableName] = entity.TableName,
            [PlaceholderConstants.PrimaryKey] = entity.PrimaryKey,
            [PlaceholderConstants.PrimaryKeyType] = entity.PrimaryKeyType,
            [PlaceholderConstants.Properties] = BuildProperties(entity.Properties),
            [PlaceholderConstants.Columns] = BuildColumns(entity.Properties),
            [PlaceholderConstants.ParameterList] = BuildParameterList(entity.Properties),
            [PlaceholderConstants.RepositoryName] = $"{entity.EntityName}Repository",
            [PlaceholderConstants.ServiceName] = $"{entity.EntityName}Service",
            [PlaceholderConstants.ManagerName] = $"{entity.EntityName}Manager",
            [PlaceholderConstants.ControllerName] = $"{entity.EntityName}Controller",
            [PlaceholderConstants.DtoName] = $"{entity.EntityName}Dto",
            [PlaceholderConstants.StoredProcedureName] = entity.EntityName
        };

        AddOptional(values, PlaceholderConstants.CompanyName, entity.CompanyName);
        AddOptional(values, PlaceholderConstants.Author, entity.Author);
        AddOptional(values, PlaceholderConstants.BaseClass, entity.BaseClass);
        AddOptional(values, PlaceholderConstants.Interfaces, string.Join(", ", entity.Interfaces));
        AddOptional(values, PlaceholderConstants.Date, DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"));

        return values;
    }

    private static void AddOptional(
        IDictionary<string, string> values,
        string placeholder,
        string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            values[placeholder] = value;
        }
    }

    private static string BuildProperties(IEnumerable<PropertyDefinition> properties)
    {
        return string.Join(
            Environment.NewLine,
            properties
                .OrderBy(property => property.Order)
                .Select(property => $"    public {BuildTypeName(property)} {property.Name} {{ get; init; }}"));
    }

    private static string BuildColumns(IEnumerable<PropertyDefinition> properties)
    {
        return string.Join(
            $",{Environment.NewLine}",
            properties
                .OrderBy(property => property.Order)
                .Select(property => $"        [{GetColumnName(property)}]"));
    }

    private static string BuildParameterList(IEnumerable<PropertyDefinition> properties)
    {
        return string.Join(
            Environment.NewLine,
            properties
                .Where(property => !property.IsIdentity)
                .OrderBy(property => property.Order)
                .Select(property => $"        parameters.Add(\"@{GetColumnName(property)}\", entity.{property.Name});"));
    }

    private static string BuildTypeName(PropertyDefinition property)
    {
        if (!property.IsNullable || property.Type.EndsWith("?", StringComparison.Ordinal))
        {
            return property.Type;
        }

        return $"{property.Type}?";
    }

    private static string GetColumnName(PropertyDefinition property)
    {
        return string.IsNullOrWhiteSpace(property.ColumnName)
            ? property.Name
            : property.ColumnName;
    }
}
