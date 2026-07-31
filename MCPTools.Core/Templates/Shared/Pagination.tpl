namespace {{Namespace}}.Shared;

/// <summary>
/// Represents pagination input.
/// </summary>
public sealed class Pagination
{
    /// <summary>
    /// Gets the requested page number.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Gets the requested page size.
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Gets the zero-based offset.
    /// </summary>
    public int Offset => (PageNumber - 1) * PageSize;
}
