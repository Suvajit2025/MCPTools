namespace {{Namespace}}.Shared;

/// <summary>
/// Represents a consistent API response.
/// </summary>
public sealed class ApiResponse<TValue>
{
    /// <summary>
    /// Gets a value indicating whether the request succeeded.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the response data.
    /// </summary>
    public TValue? Data { get; init; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the response errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    public static ApiResponse<TValue> Ok(TValue data, string? message = null)
    {
        return new ApiResponse<TValue> { Success = true, Data = data, Message = message };
    }

    /// <summary>
    /// Creates a failed API response.
    /// </summary>
    public static ApiResponse<TValue> Failure(string message, IReadOnlyList<string> errors)
    {
        return new ApiResponse<TValue> { Success = false, Message = message, Errors = errors };
    }
}
