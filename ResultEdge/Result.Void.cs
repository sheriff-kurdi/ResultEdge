namespace ResultEdge;

public class Result : Result<Result>
{
    public Result() : base() { }

    protected internal Result(ResultStatus status) : base(status) { }

    /// <summary>Successful operation with no return value.</summary>
    public static Result Success() => new();

    /// <summary>Successful operation with no return value and a success message.</summary>
    public static Result SuccessWithMessage(string successMessage) =>
        new() { SuccessMessage = successMessage };

    /// <summary>Successful operation returning a typed data value.</summary>
    public static Result<T> Success<T>(T data) => new(data);

    /// <summary>Successful operation returning a typed data value with a success message.</summary>
    public static Result<T> Success<T>(T data, string successMessage) => new(data, successMessage);

    /// <summary>General application error (HTTP 500).</summary>
    public new static Result Error(params string[] errorMessages) =>
        new(ResultStatus.Error) { Errors = errorMessages };

    /// <summary>General application error with a correlation ID for distributed tracing.</summary>
    public new static Result ErrorWithCorrelationId(string correlationId, params string[] errorMessages) =>
        new(ResultStatus.Error) { CorrelationId = correlationId, Errors = errorMessages };

    /// <summary>Validation failure with a single error (HTTP 422).</summary>
    public new static Result Invalid(ValidationError validationError) =>
        new(ResultStatus.Invalid) { ValidationErrors = [validationError] };

    /// <summary>Validation failure with multiple errors (HTTP 422).</summary>
    public new static Result Invalid(params ValidationError[] validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = validationErrors };

    /// <summary>Validation failure with a read-only list of errors (HTTP 422).</summary>
    public new static Result Invalid(IReadOnlyList<ValidationError> validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = validationErrors };

    /// <summary>Resource not found (HTTP 404).</summary>
    public new static Result NotFound() => new(ResultStatus.NotFound);

    /// <summary>Resource not found with error messages (HTTP 404).</summary>
    public new static Result NotFound(params string[] errorMessages) =>
        new(ResultStatus.NotFound) { Errors = errorMessages };

    /// <summary>Authenticated but not authorised (HTTP 403).</summary>
    public new static Result Forbidden() => new(ResultStatus.Forbidden);

    /// <summary>Not authenticated (HTTP 401).</summary>
    public new static Result Unauthorized() => new(ResultStatus.Unauthorized);

    /// <summary>State conflict (HTTP 409).</summary>
    public new static Result Conflict() => new(ResultStatus.Conflict);

    /// <summary>State conflict with error messages (HTTP 409).</summary>
    public new static Result Conflict(params string[] errorMessages) =>
        new(ResultStatus.Conflict) { Errors = errorMessages };

    /// <summary>Unhandled exception or internal server error (HTTP 500).</summary>
    public new static Result CriticalError(params string[] errorMessages) =>
        new(ResultStatus.CriticalError) { Errors = errorMessages };

    /// <summary>Dependency unavailable — transient, caller may retry (HTTP 503).</summary>
    public new static Result Unavailable(params string[] errorMessages) =>
        new(ResultStatus.Unavailable) { Errors = errorMessages };
}
