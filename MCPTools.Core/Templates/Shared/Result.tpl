namespace {{Namespace}}.Shared;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    protected Result(bool success, IReadOnlyList<string> errors)
    {
        Success = success;
        Errors = errors;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets operation errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Ok()
    {
        return new Result(true, []);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static Result Fail(params string[] errors)
    {
        return new Result(false, errors);
    }
}

/// <summary>
/// Represents the result of an operation with a value.
/// </summary>
public sealed class Result<TValue> : Result
{
    private Result(bool success, TValue? value, IReadOnlyList<string> errors)
        : base(success, errors)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the operation value.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result<TValue> Ok(TValue value)
    {
        return new Result<TValue>(true, value, []);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static new Result<TValue> Fail(params string[] errors)
    {
        return new Result<TValue>(false, default, errors);
    }
}
