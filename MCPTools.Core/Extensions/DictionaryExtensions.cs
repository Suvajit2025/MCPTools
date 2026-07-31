namespace MCPTools.Core.Extensions;

/// <summary>
/// Provides dictionary helper extension methods used by MCPTools.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the value associated with the specified key or a default value when the key is not found.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="dictionary">The dictionary to search.</param>
    /// <param name="key">The key to locate.</param>
    /// <param name="defaultValue">The value to return when the key is not found.</param>
    /// <returns>The value associated with the key, or <paramref name="defaultValue"/> when the key is not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dictionary"/> is <see langword="null"/>.</exception>
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue? defaultValue = default)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Determines whether the dictionary contains the specified template placeholder.
    /// </summary>
    /// <param name="dictionary">The dictionary to search.</param>
    /// <param name="placeholder">The placeholder to locate.</param>
    /// <returns><see langword="true"/> when the placeholder exists; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dictionary"/> or <paramref name="placeholder"/> is <see langword="null"/>.</exception>
    public static bool ContainsPlaceholder(
        this IReadOnlyDictionary<string, string> dictionary,
        string placeholder)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(placeholder);

        return dictionary.ContainsKey(placeholder);
    }
}
