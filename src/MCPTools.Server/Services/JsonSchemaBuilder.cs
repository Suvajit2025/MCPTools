using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MCPTools.Server.Services;

/// <summary>
/// Builds JSON Schema documents from CLR request and response model types.
/// </summary>
public sealed class JsonSchemaBuilder
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Builds a JSON Schema Draft 2020-12 document for the specified CLR type.
    /// </summary>
    /// <param name="type">The CLR type to describe.</param>
    /// <returns>A JSON Schema document.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    public JsonElement BuildSchema(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var schema = BuildSchemaNode(type, []);
        var schemaObject = schema.AsObject();
        schemaObject["$schema"] = "https://json-schema.org/draft/2020-12/schema";

        return JsonSerializer.SerializeToElement(schemaObject, SerializerOptions);
    }

    /// <summary>
    /// Builds a JSON Schema fragment for the specified property CLR type.
    /// </summary>
    /// <param name="type">The CLR type to describe.</param>
    /// <returns>A JSON Schema fragment.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    public JsonElement BuildPropertySchema(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var schema = BuildSchemaNode(type, []);

        return JsonSerializer.SerializeToElement(schema, SerializerOptions);
    }

    private static JsonNode BuildSchemaNode(Type type, HashSet<Type> visitedTypes)
    {
        var nullableType = Nullable.GetUnderlyingType(type);

        if (nullableType is not null)
        {
            var nullableSchema = BuildSchemaNode(nullableType, visitedTypes).AsObject();
            AddNullType(nullableSchema);
            return nullableSchema;
        }

        if (TryBuildPrimitiveSchema(type, out var primitiveSchema))
        {
            return primitiveSchema;
        }

        if (type.IsEnum)
        {
            return BuildEnumSchema(type);
        }

        var dictionaryValueType = GetDictionaryValueType(type);

        if (dictionaryValueType is not null)
        {
            return new JsonObject
            {
                ["type"] = "object",
                ["additionalProperties"] = BuildSchemaNode(dictionaryValueType, visitedTypes)
            };
        }

        var enumerableElementType = GetEnumerableElementType(type);

        if (enumerableElementType is not null)
        {
            return new JsonObject
            {
                ["type"] = "array",
                ["items"] = BuildSchemaNode(enumerableElementType, visitedTypes)
            };
        }

        return BuildObjectSchema(type, visitedTypes);
    }

    private static JsonObject BuildObjectSchema(Type type, HashSet<Type> visitedTypes)
    {
        if (!visitedTypes.Add(type))
        {
            return new JsonObject
            {
                ["type"] = "object",
                ["additionalProperties"] = true
            };
        }

        var properties = new JsonObject();
        var required = new JsonArray();

        foreach (var property in GetSerializableProperties(type))
        {
            var jsonPropertyName = SerializerOptions.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
            properties[jsonPropertyName] = BuildSchemaNode(property.PropertyType, visitedTypes);

            if (IsRequired(property))
            {
                required.Add(jsonPropertyName);
            }
        }

        visitedTypes.Remove(type);

        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["required"] = required
        };
    }

    private static JsonObject BuildEnumSchema(Type type)
    {
        return new JsonObject
        {
            ["type"] = "string",
            ["enum"] = new JsonArray(Enum.GetNames(type).Select(name => JsonValue.Create(name)).ToArray<JsonNode?>())
        };
    }

    private static bool TryBuildPrimitiveSchema(Type type, out JsonObject schema)
    {
        schema = type switch
        {
            _ when type == typeof(string) || type == typeof(char) => new JsonObject { ["type"] = "string" },
            _ when type == typeof(bool) => new JsonObject { ["type"] = "boolean" },
            _ when IsIntegerType(type) => new JsonObject { ["type"] = "integer" },
            _ when IsNumberType(type) => new JsonObject { ["type"] = "number" },
            _ when type == typeof(DateTime) || type == typeof(DateTimeOffset) => new JsonObject
            {
                ["type"] = "string",
                ["format"] = "date-time"
            },
            _ when type == typeof(DateOnly) => new JsonObject
            {
                ["type"] = "string",
                ["format"] = "date"
            },
            _ when type == typeof(TimeOnly) || type == typeof(TimeSpan) => new JsonObject
            {
                ["type"] = "string",
                ["format"] = "time"
            },
            _ when type == typeof(Guid) => new JsonObject
            {
                ["type"] = "string",
                ["format"] = "uuid"
            },
            _ => []
        };

        return schema.Count > 0;
    }

    private static IEnumerable<PropertyInfo> GetSerializableProperties(Type type)
    {
        return type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetMethod is not null && property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static Type? GetDictionaryValueType(Type type)
    {
        var dictionaryType = GetGenericType(type, typeof(IDictionary<,>))
            ?? GetGenericType(type, typeof(IReadOnlyDictionary<,>));

        if (dictionaryType is null)
        {
            return null;
        }

        var genericArguments = dictionaryType.GetGenericArguments();

        return genericArguments[0] == typeof(string) ? genericArguments[1] : null;
    }

    private static Type? GetEnumerableElementType(Type type)
    {
        if (type == typeof(string))
        {
            return null;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        var enumerableType = GetGenericType(type, typeof(IEnumerable<>));

        return enumerableType?.GetGenericArguments()[0];
    }

    private static Type? GetGenericType(Type type, Type genericTypeDefinition)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
        {
            return type;
        }

        return type
            .GetInterfaces()
            .FirstOrDefault(interfaceType => interfaceType.IsGenericType
                && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition);
    }

    private static bool IsRequired(PropertyInfo property)
    {
        return property.GetCustomAttribute<RequiredMemberAttribute>() is not null
            || property.GetCustomAttributes().Any(attribute =>
                string.Equals(
                    attribute.GetType().FullName,
                    "System.ComponentModel.DataAnnotations.RequiredAttribute",
                    StringComparison.Ordinal));
    }

    private static void AddNullType(JsonObject schema)
    {
        if (!schema.TryGetPropertyValue("type", out var typeNode) || typeNode is null)
        {
            return;
        }

        if (typeNode is JsonArray jsonArray)
        {
            if (!jsonArray.Any(node => node?.GetValue<string>() == "null"))
            {
                jsonArray.Add("null");
            }

            return;
        }

        var typeName = typeNode.GetValue<string>();

        if (typeName != "null")
        {
            schema["type"] = new JsonArray(typeName, "null");
        }
    }

    private static bool IsIntegerType(Type type)
    {
        return type == typeof(byte)
            || type == typeof(sbyte)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(long)
            || type == typeof(ulong);
    }

    private static bool IsNumberType(Type type)
    {
        return type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal);
    }
}
