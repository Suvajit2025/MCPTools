namespace {{Namespace}}.Shared;

/// <summary>
/// Represents a paged result set.
/// </summary>
public sealed class PagedResult<TItem>
{
    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total item count.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the total page count.
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; init; } = [];
}
