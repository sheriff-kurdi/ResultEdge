#if !NETSTANDARD2_0
using System.Text.Json.Serialization;
#endif

namespace ResultEdge;

public class Result<T> : IResult
{
    protected Result() { }

    public Result(T data)
    {
        Data = data;
    }

    protected internal Result(T data, string successMessage) : this(data)
    {
        SuccessMessage = successMessage;
    }

    protected Result(ResultStatus status)
    {
        Status = status;
    }

    public static implicit operator T(Result<T> result) => result.Data!;
    public static implicit operator Result<T>(T data) => new(data);

    public static implicit operator Result<T>(Result result) => new Result<T>(default(T)!)
    {
        Status = result.Status,
        Errors = result.Errors,
        SuccessMessage = result.SuccessMessage,
        CorrelationId = result.CorrelationId,
        ValidationErrors = result.ValidationErrors,
    };

    public T? Data { get; }

#if !NETSTANDARD2_0
    [JsonIgnore]
#endif
    public Type DataType => typeof(T);
    public ResultStatus Status { get; protected set; } = ResultStatus.Ok;
    public bool IsSuccess => Status == ResultStatus.Ok;
    public bool IsFailure => !IsSuccess;
    public string SuccessMessage { get; protected internal set; } = string.Empty;
    public string CorrelationId { get; protected internal set; } = string.Empty;
    public IEnumerable<string> Errors { get; protected set; } = new List<string>();
    public IReadOnlyList<ValidationError> ValidationErrors { get; protected set; } = [];

    public object? GetData() => Data;

    /// <summary>
    /// Converts this result into a PagedResult&lt;T&gt; with the supplied pagination metadata.
    /// </summary>
    public PagedResult<T> ToPagedResult(PagedInfo pagedInfo)
    {
        return new PagedResult<T>(pagedInfo, Data!)
        {
            Status = Status,
            SuccessMessage = SuccessMessage,
            CorrelationId = CorrelationId,
            Errors = Errors,
            ValidationErrors = ValidationErrors
        };
    }

    /// <summary>Successful result carrying a data value.</summary>
    public static Result<T> Success(T data) => new(data);

    /// <summary>Successful result carrying a data value and a success message.</summary>
    public static Result<T> Success(T data, string successMessage) => new(data, successMessage);

    /// <summary>General application error (HTTP 500).</summary>
    public static Result<T> Error(params string[] errorMessages) =>
        new(ResultStatus.Error) { Errors = errorMessages };

    /// <summary>General application error with a correlation ID for distributed tracing.</summary>
    public static Result<T> ErrorWithCorrelationId(string correlationId, params string[] errorMessages) =>
        new(ResultStatus.Error) { CorrelationId = correlationId, Errors = errorMessages };

    /// <summary>Validation failure with a single error (HTTP 422).</summary>
    public static Result<T> Invalid(ValidationError validationError) =>
        new(ResultStatus.Invalid) { ValidationErrors = [validationError] };

    /// <summary>Validation failure with multiple errors (HTTP 422).</summary>
    public static Result<T> Invalid(params ValidationError[] validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = validationErrors };

    /// <summary>Validation failure with a read-only list of errors (HTTP 422).</summary>
    public static Result<T> Invalid(IReadOnlyList<ValidationError> validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = validationErrors };

    /// <summary>Resource not found (HTTP 404).</summary>
    public static Result<T> NotFound() => new(ResultStatus.NotFound);

    /// <summary>Resource not found with error messages (HTTP 404).</summary>
    public static Result<T> NotFound(params string[] errorMessages) =>
        new(ResultStatus.NotFound) { Errors = errorMessages };

    /// <summary>Authenticated but not authorised (HTTP 403).</summary>
    public static Result<T> Forbidden() => new(ResultStatus.Forbidden);

    /// <summary>Not authenticated (HTTP 401).</summary>
    public static Result<T> Unauthorized() => new(ResultStatus.Unauthorized);

    /// <summary>State conflict (HTTP 409).</summary>
    public static Result<T> Conflict() => new(ResultStatus.Conflict);

    /// <summary>State conflict with error messages (HTTP 409).</summary>
    public static Result<T> Conflict(params string[] errorMessages) =>
        new(ResultStatus.Conflict) { Errors = errorMessages };

    /// <summary>Unhandled exception or internal server error (HTTP 500).</summary>
    public static Result<T> CriticalError(params string[] errorMessages) =>
        new(ResultStatus.CriticalError) { Errors = errorMessages };

    /// <summary>Dependency unavailable — transient, caller may retry (HTTP 503).</summary>
    public static Result<T> Unavailable(params string[] errorMessages) =>
        new(ResultStatus.Unavailable) { Errors = errorMessages };
}
