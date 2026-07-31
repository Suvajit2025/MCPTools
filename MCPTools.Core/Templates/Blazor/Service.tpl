namespace {{Namespace}}.Client.Services;

using System.Net.Http.Json;

/// <summary>
/// Provides client-side API access for {{EntityName}}.
/// </summary>
public sealed class {{EntityName}}Service
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{EntityName}}Service"/> class.
    /// </summary>
    public {{EntityName}}Service(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Gets all {{PluralEntityName}}.
    /// </summary>
    public async Task<IReadOnlyList<{{EntityName}}Model>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<{{EntityName}}Model>>(
            "/api/v{{ApiVersion}}/{{Route}}",
            cancellationToken);

        return result ?? [];
    }
}
