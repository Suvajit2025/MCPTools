namespace {{Namespace}}.Application.Mapping;

using {{Namespace}}.Application.Requests;
using {{Namespace}}.Application.Responses;
using {{Namespace}}.Domain.Entities;

/// <summary>
/// Maps {{EntityName}} objects between domain and application contracts.
/// </summary>
public static class {{EntityName}}Mapper
{
    /// <summary>
    /// Maps a domain entity to a response model.
    /// </summary>
    public static {{EntityName}}Response ToResponse({{EntityName}} entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new {{EntityName}}Response
        {
{{Properties}}
        };
    }

    /// <summary>
    /// Maps a create request to a domain entity.
    /// </summary>
    public static {{EntityName}} FromCreateRequest(Create{{EntityName}}Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new {{EntityName}}
        {
{{Properties}}
        };
    }

    /// <summary>
    /// Maps an update request to a domain entity.
    /// </summary>
    public static {{EntityName}} FromUpdateRequest(Update{{EntityName}}Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new {{EntityName}}
        {
{{Properties}}
        };
    }
}
