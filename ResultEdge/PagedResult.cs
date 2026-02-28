namespace ResultEdge;

public class PagedResult<T> : Result<T>
{
    public PagedResult(PagedInfo pagedInfo, T data) : base(data)
    {
        PagedInfo = pagedInfo;
    }

    private PagedResult(ResultStatus status) : base(status) { }

    public PagedInfo PagedInfo { get; } = default!;

    public new static PagedResult<T> Error(params string[] errorMessages) =>
        new(ResultStatus.Error) { Errors = errorMessages };

    public new static PagedResult<T> Invalid(ValidationError validationError) =>
        new(ResultStatus.Invalid) { ValidationErrors = { validationError } };

    public new static PagedResult<T> Invalid(params ValidationError[] validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = [.. validationErrors] };

    public new static PagedResult<T> Invalid(List<ValidationError> validationErrors) =>
        new(ResultStatus.Invalid) { ValidationErrors = validationErrors };

    public new static PagedResult<T> NotFound() =>
        new(ResultStatus.NotFound);

    public new static PagedResult<T> NotFound(params string[] errorMessages) =>
        new(ResultStatus.NotFound) { Errors = errorMessages };

    public new static PagedResult<T> Forbidden() =>
        new(ResultStatus.Forbidden);

    public new static PagedResult<T> Unauthorized() =>
        new(ResultStatus.Unauthorized);

    public new static PagedResult<T> Conflict() =>
        new(ResultStatus.Conflict);

    public new static PagedResult<T> Conflict(params string[] errorMessages) =>
        new(ResultStatus.Conflict) { Errors = errorMessages };

    public new static PagedResult<T> CriticalError(params string[] errorMessages) =>
        new(ResultStatus.CriticalError) { Errors = errorMessages };

    public new static PagedResult<T> Unavailable(params string[] errorMessages) =>
        new(ResultStatus.Unavailable) { Errors = errorMessages };
}
