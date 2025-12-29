using ResultEdge;

namespace ResultEdge.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void Map_WithSuccessResult_ShouldTransformValue()
    {
        var result = Result<int>.Success(42);

        var mappedResult = result.Map(x => x.ToString());

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal("42", mappedResult.Value);
        Assert.Equal(ResultStatus.Ok, mappedResult.Status);
    }

    [Fact]
    public void Map_WithSuccessResult_ShouldInvokeFunc()
    {
        var result = Result<string>.Success("hello");

        var mappedResult = result.Map(x => x.ToUpper());

        Assert.Equal("HELLO", mappedResult.Value);
    }

    [Fact]
    public void Map_WithComplexTransformation_ShouldWork()
    {
        var result = Result<int>.Success(5);

        var mappedResult = result.Map(x => new { Value = x, Square = x * x });

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(5, mappedResult.Value!.Value);
        Assert.Equal(25, mappedResult.Value!.Square);
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
        Assert.Equal("25", mappedResult.Value);
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
    public void Map_FromStringToInt_ShouldWork()
    {
        var result = Result<string>.Success("123");

        var mappedResult = result.Map(x => int.Parse(x));

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(123, mappedResult.Value);
    }

    [Fact]
    public void Map_ToComplexObject_ShouldWork()
    {
        var result = Result<int>.Success(42);

        var mappedResult = result.Map(x => new List<int> { x, x * 2, x * 3 });

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(3, mappedResult.Value!.Count);
        Assert.Equal(42, mappedResult.Value[0]);
        Assert.Equal(84, mappedResult.Value[1]);
        Assert.Equal(126, mappedResult.Value[2]);
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
}
