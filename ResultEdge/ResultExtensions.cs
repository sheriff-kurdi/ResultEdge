namespace ResultEdge;

public static class ResultExtensions
{
    /// <summary>
    /// Transforms the Data of a successful result from <typeparamref name="TSource"/> to
    /// <typeparamref name="TDestination"/>. Non-success statuses, errors, SuccessMessage,
    /// and CorrelationId are all propagated automatically without invoking <paramref name="func"/>.
    /// </summary>
    public static Result<TDestination> Map<TSource, TDestination>(
        this Result<TSource> result,
        Func<TSource, TDestination> func)
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
                return new Result<TDestination>(func(result.Data!))
                {
                    SuccessMessage = result.SuccessMessage,
                    CorrelationId = result.CorrelationId
                };
            case ResultStatus.NotFound:
                return result.Errors.Any()
                    ? Result<TDestination>.NotFound(result.Errors.ToArray())
                    : Result<TDestination>.NotFound();
            case ResultStatus.Unauthorized:
                return Result<TDestination>.Unauthorized();
            case ResultStatus.Forbidden:
                return Result<TDestination>.Forbidden();
            case ResultStatus.Invalid:
                return Result<TDestination>.Invalid(result.ValidationErrors);
            case ResultStatus.Error:
                return result.CorrelationId.Length > 0
                    ? Result<TDestination>.ErrorWithCorrelationId(result.CorrelationId, result.Errors.ToArray())
                    : Result<TDestination>.Error(result.Errors.ToArray());
            case ResultStatus.Conflict:
                return result.Errors.Any()
                    ? Result<TDestination>.Conflict(result.Errors.ToArray())
                    : Result<TDestination>.Conflict();
            case ResultStatus.CriticalError:
                return Result<TDestination>.CriticalError(result.Errors.ToArray());
            case ResultStatus.Unavailable:
                return Result<TDestination>.Unavailable(result.Errors.ToArray());
            default:
                throw new NotSupportedException($"Result status {result.Status} is not supported.");
        }
    }

    /// <summary>
    /// Asynchronously transforms the Data of a successful result.
    /// Non-success statuses are propagated without invoking <paramref name="func"/>.
    /// </summary>
    public static async Task<Result<TDestination>> MapAsync<TSource, TDestination>(
        this Result<TSource> result,
        Func<TSource, Task<TDestination>> func)
    {
        if (result.IsFailure)
            return result.Map(_ => default(TDestination)!);

        var data = await func(result.Data!).ConfigureAwait(false);
        return new Result<TDestination>(data)
        {
            SuccessMessage = result.SuccessMessage,
            CorrelationId = result.CorrelationId
        };
    }

    /// <summary>
    /// Awaits a result task then synchronously transforms the Data.
    /// </summary>
    public static async Task<Result<TDestination>> MapAsync<TSource, TDestination>(
        this Task<Result<TSource>> resultTask,
        Func<TSource, TDestination> func)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Map(func);
    }

    /// <summary>
    /// Awaits a result task then asynchronously transforms the Data.
    /// </summary>
    public static async Task<Result<TDestination>> MapAsync<TSource, TDestination>(
        this Task<Result<TSource>> resultTask,
        Func<TSource, Task<TDestination>> func)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapAsync(func).ConfigureAwait(false);
    }

    /// <summary>
    /// Chains an operation that itself returns a <see cref="Result{TDestination}"/>.
    /// If the current result is not successful, its status and errors are propagated
    /// without calling <paramref name="func"/>.
    /// </summary>
    public static Result<TDestination> Bind<TSource, TDestination>(
        this Result<TSource> result,
        Func<TSource, Result<TDestination>> func)
    {
        if (result.IsFailure)
            return result.Map(_ => default(TDestination)!);

        return func(result.Data!);
    }

    /// <summary>
    /// Asynchronously chains an operation that returns a <see cref="Result{TDestination}"/>.
    /// </summary>
    public static async Task<Result<TDestination>> BindAsync<TSource, TDestination>(
        this Result<TSource> result,
        Func<TSource, Task<Result<TDestination>>> func)
    {
        if (result.IsFailure)
            return result.Map(_ => default(TDestination)!);

        return await func(result.Data!).ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits a result task then synchronously chains an operation.
    /// </summary>
    public static async Task<Result<TDestination>> BindAsync<TSource, TDestination>(
        this Task<Result<TSource>> resultTask,
        Func<TSource, Result<TDestination>> func)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(func);
    }

    /// <summary>
    /// Awaits a result task then asynchronously chains an operation.
    /// </summary>
    public static async Task<Result<TDestination>> BindAsync<TSource, TDestination>(
        this Task<Result<TSource>> resultTask,
        Func<TSource, Task<Result<TDestination>>> func)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(func).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes <paramref name="onSuccess"/> with the Data when the result is successful,
    /// otherwise executes <paramref name="onFailure"/> with the full result.
    /// Returns the value produced by whichever branch executes.
    /// </summary>
    public static TResult Match<T, TResult>(
        this Result<T> result,
        Func<T?, TResult> onSuccess,
        Func<Result<T>, TResult> onFailure) =>
        result.IsSuccess ? onSuccess(result.Data) : onFailure(result);

    /// <summary>
    /// Executes <paramref name="onSuccess"/> when the result is successful,
    /// otherwise executes <paramref name="onFailure"/>. Returns no value.
    /// </summary>
    public static void Match<T>(
        this Result<T> result,
        Action<T?> onSuccess,
        Action<Result<T>> onFailure)
    {
        if (result.IsSuccess)
            onSuccess(result.Data);
        else
            onFailure(result);
    }

    /// <summary>
    /// Executes <paramref name="onSuccess"/> when the void result is successful,
    /// otherwise executes <paramref name="onFailure"/>.
    /// Returns the value produced by whichever branch executes.
    /// </summary>
    public static TResult Match<TResult>(
        this Result result,
        Func<TResult> onSuccess,
        Func<Result, TResult> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result);

    /// <summary>
    /// Executes <paramref name="onSuccess"/> when the void result is successful,
    /// otherwise executes <paramref name="onFailure"/>. Returns no value.
    /// </summary>
    public static void Match(
        this Result result,
        Action onSuccess,
        Action<Result> onFailure)
    {
        if (result.IsSuccess)
            onSuccess();
        else
            onFailure(result);
    }
}
