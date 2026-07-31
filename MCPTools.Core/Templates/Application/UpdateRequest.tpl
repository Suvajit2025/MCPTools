namespace {{Namespace}}.Application.Requests;

/// <summary>
/// Represents the request used to update a {{EntityName}}.
/// </summary>
public sealed record Update{{EntityName}}Request
{
    /// <summary>
    /// Gets the {{EntityName}} primary key.
    /// </summary>
    public required {{PrimaryKeyType}} {{PrimaryKey}} { get; init; }

{{Properties}}
}
