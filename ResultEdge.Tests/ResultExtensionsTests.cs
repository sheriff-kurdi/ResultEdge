using ResultEdge;

namespace ResultEdge.Tests;

public class ResultExtensionsTests
{
    // ── Map ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Map_WithSuccessResult_ShouldTransformData()
    {
        var result = Result<int>.Success(42);

        var mappedResult = result.Map(x => x.ToString());

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal("42", mappedResult.Data);
        Assert.Equal(ResultStatus.Ok, mappedResult.Status);
    }

    [Fact]
    public void Map_WithSuccessResult_ShouldInvokeFunc()
    {
        var result = Result<string>.Success("hello");

        var mappedResult = result.Map(x => x.ToUpper());

        Assert.Equal("HELLO", mappedResult.Data);
    }

    [Fact]
    public void Map_WithSuccessResult_ShouldPropagateSuccessMessage()
    {
        var result = Result<int>.Success(10, "Loaded");

        var mappedResult = result.Map(x => x * 2);

        Assert.Equal("Loaded", mappedResult.SuccessMessage);
    }

    [Fact]
    public void Map_WithErrorWithCorrelationId_ShouldPropagateCorrelationId()
    {
        var result = Result<int>.ErrorWithCorrelationId("corr-999", "err");

        var mappedResult = result.Map(x => x.ToString());

        Assert.Equal("corr-999", mappedResult.CorrelationId);
        Assert.Equal(ResultStatus.Error, mappedResult.Status);
    }

    [Fact]
    public void Map_WithComplexTransformation_ShouldWork()
    {
        var result = Result<int>.Success(5);

        var mappedResult = result.Map(x => new { Value = x, Square = x * x });

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(5, mappedResult.Data!.Value);
        Assert.Equal(25, mappedResult.Data!.Square);
    }

    [Fact]
    public void Map_WithErrorResult_ShouldPreserveErrors()
    {
        var result = Result<int>.Error("Error 1", "Error 2");

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Error, mappedResult.Status);
        Assert.Equal(2, mappedResult.Errors.Count());
        Assert.Contains("Error 1", mappedResult.Errors);
        Assert.Contains("Error 2", mappedResult.Errors);
    }

    [Fact]
    public void Map_WithNotFoundResult_WithoutErrors_ShouldPreserveStatus()
    {
        var result = Result<int>.NotFound();

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, mappedResult.Status);
        Assert.Empty(mappedResult.Errors);
    }

    [Fact]
    public void Map_WithNotFoundResult_WithErrors_ShouldPreserveErrors()
    {
        var result = Result<int>.NotFound("Resource not found", "ID: 123");

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, mappedResult.Status);
        Assert.Equal(2, mappedResult.Errors.Count());
        Assert.Contains("Resource not found", mappedResult.Errors);
    }

    [Fact]
    public void Map_WithUnauthorizedResult_ShouldPreserveStatus()
    {
        var result = Result<int>.Unauthorized();

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Unauthorized, mappedResult.Status);
    }

    [Fact]
    public void Map_WithForbiddenResult_ShouldPreserveStatus()
    {
        var result = Result<int>.Forbidden();

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, mappedResult.Status);
    }

    [Fact]
    public void Map_WithInvalidResult_ShouldPreserveValidationErrors()
    {
        var validationError1 = new ValidationError("Field1", "Error 1", "ERR_001", ValidationSeverity.Error);
        var validationError2 = new ValidationError("Field2", "Error 2", "ERR_002", ValidationSeverity.Warning);
        var result = Result<int>.Invalid(validationError1, validationError2);

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, mappedResult.Status);
        Assert.Equal(2, mappedResult.ValidationErrors.Count);
        Assert.Equal("Field1", mappedResult.ValidationErrors[0].Identifier);
        Assert.Equal("Field2", mappedResult.ValidationErrors[1].Identifier);
    }

    [Fact]
    public void Map_WithConflictResult_WithoutErrors_ShouldPreserveStatus()
    {
        var result = Result<int>.Conflict();

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, mappedResult.Status);
        Assert.Empty(mappedResult.Errors);
    }

    [Fact]
    public void Map_WithConflictResult_WithErrors_ShouldPreserveErrors()
    {
        var result = Result<int>.Conflict("Version mismatch", "Resource modified");

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, mappedResult.Status);
        Assert.Equal(2, mappedResult.Errors.Count());
        Assert.Contains("Version mismatch", mappedResult.Errors);
    }

    [Fact]
    public void Map_WithCriticalErrorResult_ShouldPreserveErrors()
    {
        var result = Result<int>.CriticalError("Database failure", "Connection lost");

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.CriticalError, mappedResult.Status);
        Assert.Equal(2, mappedResult.Errors.Count());
        Assert.Contains("Database failure", mappedResult.Errors);
    }

    [Fact]
    public void Map_WithUnavailableResult_ShouldPreserveErrors()
    {
        var result = Result<int>.Unavailable("Service unavailable", "Retry later");

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Unavailable, mappedResult.Status);
        Assert.Equal(2, mappedResult.Errors.Count());
        Assert.Contains("Service unavailable", mappedResult.Errors);
    }

    [Fact]
    public void Map_ChainedTransformations_ShouldWork()
    {
        var result = Result<int>.Success(10);

        var mappedResult = result
            .Map(x => x * 2)
            .Map(x => x + 5)
            .Map(x => x.ToString());

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal("25", mappedResult.Data);
    }

    [Fact]
    public void Map_ChainedWithError_ShouldStopAtError()
    {
        var result = Result<int>.Error("Initial error");

        var mappedResult = result
            .Map(x => x * 2)
            .Map(x => x + 5)
            .Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Error, mappedResult.Status);
        Assert.Contains("Initial error", mappedResult.Errors);
    }

    [Fact]
    public void Map_WithEmptyErrorCollection_ShouldHandleCorrectly()
    {
        var result = Result<int>.Error();

        var mappedResult = result.Map(x => x.ToString());

        Assert.False(mappedResult.IsSuccess);
        Assert.Equal(ResultStatus.Error, mappedResult.Status);
        Assert.Empty(mappedResult.Errors);
    }

    // ── Bind ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Bind_WithSuccessResult_ShouldInvokeFunc()
    {
        var result = Result<int>.Success(10);

        var bound = result.Bind(x => Result<string>.Success(x.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("10", bound.Data);
    }

    [Fact]
    public void Bind_WithErrorResult_ShouldPropagateAndNotInvokeFunc()
    {
        var funcInvoked = false;
        var result = Result<int>.Error("upstream error");

        var bound = result.Bind(x =>
        {
            funcInvoked = true;
            return Result<string>.Success(x.ToString());
        });

        Assert.False(funcInvoked);
        Assert.False(bound.IsSuccess);
        Assert.Equal(ResultStatus.Error, bound.Status);
        Assert.Contains("upstream error", bound.Errors);
    }

    [Fact]
    public void Bind_WhenFuncReturnsFailure_ShouldReturnFailure()
    {
        var result = Result<int>.Success(42);

        var bound = result.Bind(_ => Result<string>.NotFound("not found downstream"));

        Assert.False(bound.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, bound.Status);
        Assert.Contains("not found downstream", bound.Errors);
    }

    [Fact]
    public void Bind_Chained_ShouldComposePipeline()
    {
        var result = Result<int>.Success(5);

        var bound = result
            .Bind(x => Result<int>.Success(x * 2))
            .Bind(x => Result<string>.Success($"Value: {x}"));

        Assert.True(bound.IsSuccess);
        Assert.Equal("Value: 10", bound.Data);
    }

    [Fact]
    public void Bind_PropagatesNotFound()
    {
        var result = Result<int>.NotFound("item not found");

        var bound = result.Bind(x => Result<string>.Success(x.ToString()));

        Assert.Equal(ResultStatus.NotFound, bound.Status);
        Assert.Contains("item not found", bound.Errors);
    }

    // ── MapAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task MapAsync_WithSuccessResult_ShouldTransformData()
    {
        var result = Result<int>.Success(21);

        var mapped = await result.MapAsync(async x =>
        {
            await Task.Yield();
            return x * 2;
        });

        Assert.True(mapped.IsSuccess);
        Assert.Equal(42, mapped.Data);
    }

    [Fact]
    public async Task MapAsync_WithErrorResult_ShouldPropagateAndNotInvokeFunc()
    {
        var funcInvoked = false;
        var result = Result<int>.Error("async error");

        var mapped = await result.MapAsync(async x =>
        {
            funcInvoked = true;
            await Task.Yield();
            return x.ToString();
        });

        Assert.False(funcInvoked);
        Assert.False(mapped.IsSuccess);
        Assert.Contains("async error", mapped.Errors);
    }

    [Fact]
    public async Task MapAsync_OnTaskResult_ShouldTransformData()
    {
        var resultTask = Task.FromResult(Result<int>.Success(7));

        var mapped = await resultTask.MapAsync(x => x * 6);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(42, mapped.Data);
    }

    // ── BindAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task BindAsync_WithSuccessResult_ShouldInvokeFunc()
    {
        var result = Result<int>.Success(5);

        var bound = await result.BindAsync(async x =>
        {
            await Task.Yield();
            return Result<string>.Success($"got {x}");
        });

        Assert.True(bound.IsSuccess);
        Assert.Equal("got 5", bound.Data);
    }

    [Fact]
    public async Task BindAsync_WithErrorResult_ShouldPropagateAndNotInvokeFunc()
    {
        var funcInvoked = false;
        var result = Result<int>.Error("upstream");

        var bound = await result.BindAsync(async x =>
        {
            funcInvoked = true;
            await Task.Yield();
            return Result<string>.Success(x.ToString());
        });

        Assert.False(funcInvoked);
        Assert.False(bound.IsSuccess);
        Assert.Contains("upstream", bound.Errors);
    }

    [Fact]
    public async Task BindAsync_OnTaskResult_ShouldCompose()
    {
        var resultTask = Task.FromResult(Result<int>.Success(3));

        var bound = await resultTask.BindAsync(x => Result<int>.Success(x * 10));

        Assert.True(bound.IsSuccess);
        Assert.Equal(30, bound.Data);
    }

    // ── Match ────────────────────────────────────────────────────────────────

    [Fact]
    public void Match_WithSuccessResult_ShouldInvokeOnSuccess()
    {
        var result = Result<int>.Success(42);

        var output = result.Match(
            onSuccess: data => $"ok:{data}",
            onFailure: r => $"fail:{r.Status}");

        Assert.Equal("ok:42", output);
    }

    [Fact]
    public void Match_WithFailureResult_ShouldInvokeOnFailure()
    {
        var result = Result<int>.NotFound("missing");

        var output = result.Match(
            onSuccess: data => $"ok:{data}",
            onFailure: r => $"fail:{r.Status}");

        Assert.Equal("fail:NotFound", output);
    }

    [Fact]
    public void Match_VoidOverload_WithSuccess_ShouldInvokeOnSuccess()
    {
        var result = Result<int>.Success(10);
        var log = string.Empty;

        result.Match(
            onSuccess: data => { log = $"success:{data}"; },
            onFailure: r => { log = $"failure:{r.Status}"; });

        Assert.Equal("success:10", log);
    }

    [Fact]
    public void Match_VoidOverload_WithFailure_ShouldInvokeOnFailure()
    {
        var result = Result<int>.Forbidden();
        var log = string.Empty;

        result.Match(
            onSuccess: _ => { log = "success"; },
            onFailure: r => { log = $"failure:{r.Status}"; });

        Assert.Equal("failure:Forbidden", log);
    }

    // ── Match on void Result ──────────────────────────────────────────────────

    [Fact]
    public void Match_OnVoidResult_WithSuccess_ShouldInvokeOnSuccess()
    {
        var result = Result.Success();

        var output = result.Match(
            onSuccess: () => "ok",
            onFailure: r  => $"fail:{r.Status}");

        Assert.Equal("ok", output);
    }

    [Fact]
    public void Match_OnVoidResult_WithFailure_ShouldInvokeOnFailure()
    {
        var result = Result.Error("command failed");

        var output = result.Match(
            onSuccess: () => "ok",
            onFailure: r  => $"fail:{r.Status}");

        Assert.Equal("fail:Error", output);
    }

    [Fact]
    public void Match_VoidAction_OnVoidResult_WithSuccess_ShouldInvokeOnSuccess()
    {
        var result = Result.Success();
        var log = string.Empty;

        result.Match(
            onSuccess: ()  => { log = "done"; },
            onFailure: r   => { log = $"error:{r.Status}"; });

        Assert.Equal("done", log);
    }

    [Fact]
    public void Match_VoidAction_OnVoidResult_WithFailure_ShouldInvokeOnFailure()
    {
        var result = Result.NotFound("target not found");
        var log = string.Empty;

        result.Match(
            onSuccess: ()  => { log = "done"; },
            onFailure: r   => { log = $"error:{r.Status}"; });

        Assert.Equal("error:NotFound", log);
    }
}
