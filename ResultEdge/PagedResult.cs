namespace ResultEdge;

public class PagedResult<T> : Result<T>
{
    public PagedResult(PagedInfo pagedInfo, T data) : base(data)
    {
        PagedInfo = pagedInfo;
    }

    private PagedResult(ResultStatus status) : base(status) { }

    private PagedResult(PagedInfo pagedInfo, T data, string successMessage) : base(data, successMessage)
    {
        PagedInfo = pagedInfo;
    }

    /// <summary>
    /// Pagination metadata. This is null when the result represents a non-success status.
    /// Always check <see cref="Result{T}.IsSuccess"/> before accessing this property.
    /// </summary>
    public PagedInfo? PagedInfo { get; } = null;

    /// <summary>Successful paged result.</summary>
    public static PagedResult<T> Success(PagedInfo pagedInfo, T data) =>
        new(pagedInfo, data);

    /// <summary>Successful paged result with a success message.</summary>
    public static PagedResult<T> Success(PagedInfo pagedInfo, T data, string successMessage) =>
        new(pagedInfo, data, successMessage);

    /// <summary>General application error (HTTP 500).</summary>
    public new static PagedResult<T> Error(params string[] errorMessages) =>
        new(ResultStatus.Error) { Errors = errorMessages };

    /// <summary>General application error with a correlation ID for distributed tracing.</summary>
    public new static PagedResult<T> ErrorWithCorrelationId(string correlationId, params string[] errorMessages) =>
        new(ResultStatus.Error) { CorrelationId = correlationId, Errors = errorMessages };

    /// <summary>Validation failure with a single error (HTTP 422).</summary>
    public new static PagedResult<T> Invalid(ValidationError validationError) =>
        new(ResultStatus.Invalid) { ValidationErrors = [validationError] };

    /// <summary>Validation failure with multiple errors (HTTP 422).</summary>
    public new static PagedResult<T> Invalid(params ValidationError[] validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = validationErrors };

    /// <summary>Validation failure with a read-only list of errors (HTTP 422).</summary>
    public new static PagedResult<T> Invalid(IReadOnlyList<ValidationError> validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = validationErrors };

    /// <summary>Resource not found (HTTP 404).</summary>
    public new static PagedResult<T> NotFound() =>
        new(ResultStatus.NotFound);

    /// <summary>Resource not found with error messages (HTTP 404).</summary>
    public new static PagedResult<T> NotFound(params string[] errorMessages) =>
        new(ResultStatus.NotFound) { Errors = errorMessages };

    /// <summary>Authenticated but not authorised (HTTP 403).</summary>
    public new static PagedResult<T> Forbidden() =>
        new(ResultStatus.Forbidden);

    /// <summary>Not authenticated (HTTP 401).</summary>
    public new static PagedResult<T> Unauthorized() =>
        new(ResultStatus.Unauthorized);

    /// <summary>State conflict (HTTP 409).</summary>
    public new static PagedResult<T> Conflict() =>
        new(ResultStatus.Conflict);

    /// <summary>State conflict with error messages (HTTP 409).</summary>
    public new static PagedResult<T> Conflict(params string[] errorMessages) =>
        new(ResultStatus.Conflict) { Errors = errorMessages };

    /// <summary>Unhandled exception or internal server error (HTTP 500).</summary>
    public new static PagedResult<T> CriticalError(params string[] errorMessages) =>
        new(ResultStatus.CriticalError) { Errors = errorMessages };

    /// <summary>Dependency unavailable — transient, caller may retry (HTTP 503).</summary>
    public new static PagedResult<T> Unavailable(params string[] errorMessages) =>
        new(ResultStatus.Unavailable) { Errors = errorMessages };
}
