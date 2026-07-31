namespace MCPTools.Core.Extensions;

/// <summary>
/// Provides type helper extension methods used by MCPTools.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified type is a simple framework-supported type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> when the type is simple; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    public static bool IsSimpleType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var targetType = Nullable.GetUnderlyingType(type) ?? type;

        return targetType.IsPrimitive
            || targetType.IsEnum
            || targetType == typeof(string)
            || targetType == typeof(decimal)
            || targetType == typeof(DateTime)
            || targetType == typeof(DateTimeOffset)
            || targetType == typeof(TimeSpan)
            || targetType == typeof(Guid);
    }

    /// <summary>
    /// Determines whether the specified type is a nullable value type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> when the type is nullable; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    public static bool IsNullableType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return Nullable.GetUnderlyingType(type) is not null;
    }

    /// <summary>
    /// Gets a readable type name for diagnostics and generated output.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>A friendly type name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    public static string GetFriendlyName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var underlyingType = Nullable.GetUnderlyingType(type);

        if (underlyingType is not null)
        {
            return $"{underlyingType.GetFriendlyName()}?";
        }

        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var genericTypeName = type.Name[..type.Name.IndexOf('`', StringComparison.Ordinal)];
        var genericArguments = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName));

        return $"{genericTypeName}<{genericArguments}>";
    }
}
