namespace {{Namespace}}.Application.Mapping;

using {{Namespace}}.Application.Dtos;
using {{Namespace}}.Domain.Entities;

/// <summary>
/// Provides mapping helpers for {{ModelName}}.
/// </summary>
public static class {{ModelName}}Mapper
{
    /// <summary>
    /// Maps a {{ModelName}} entity to a response DTO.
    /// </summary>
    public static {{ModelName}}Response ToResponse({{ModelName}} entity)
    {
        return new {{ModelName}}Response
        {
{{Properties}}
        };
    }

    /// <summary>
    /// Maps a create request to a {{ModelName}} entity.
    /// </summary>
    public static {{ModelName}} FromCreateRequest(Create{{ModelName}}Request request)
    {
        return new {{ModelName}}
        {
{{Properties}}
        };
    }

    /// <summary>
    /// Maps an update request to a {{ModelName}} entity.
    /// </summary>
    public static {{ModelName}} FromUpdateRequest({{PrimaryKeyType}} {{PrimaryKey}}, Update{{ModelName}}Request request)
    {
        return new {{ModelName}}
        {
            {{PrimaryKey}} = {{PrimaryKey}},
{{Properties}}
        };
    }
}
