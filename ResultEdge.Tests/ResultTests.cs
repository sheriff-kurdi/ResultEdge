using ResultEdge;

namespace ResultEdge.Tests;

public class ResultTests
{
    [Fact]
    public void Success_WithValue_ShouldCreateSuccessResult()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(42, result.Value);
        Assert.Empty(result.Errors);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public void Success_WithValueAndMessage_ShouldSetSuccessMessage()
    {
        var result = Result<string>.Success("test", "Operation completed successfully");

        Assert.True(result.IsSuccess);
        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal("test", result.Value);
        Assert.Equal("Operation completed successfully", result.SuccessMessage);
    }

    [Fact]
    public void Error_WithMessages_ShouldCreateErrorResult()
    {
        var result = Result<int>.Error("Error 1", "Error 2");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Equal(2, result.Errors.Count());
        Assert.Contains("Error 1", result.Errors);
        Assert.Contains("Error 2", result.Errors);
    }

    [Fact]
    public void Invalid_WithSingleValidationError_ShouldCreateInvalidResult()
    {
        var validationError = new ValidationError("Email", "Invalid email format", "EMAIL_001", ValidationSeverity.Error);
        var result = Result<int>.Invalid(validationError);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Single(result.ValidationErrors);
        Assert.Equal("Email", result.ValidationErrors[0].Identifier);
    }

    [Fact]
    public void Invalid_WithMultipleValidationErrors_ShouldCreateInvalidResult()
    {
        var errors = new[]
        {
            new ValidationError("Email", "Invalid email", "EMAIL_001", ValidationSeverity.Error),
            new ValidationError("Phone", "Invalid phone", "PHONE_001", ValidationSeverity.Error)
        };
        var result = Result<int>.Invalid(errors);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Equal(2, result.ValidationErrors.Count);
    }

    [Fact]
    public void Invalid_WithValidationErrorList_ShouldCreateInvalidResult()
    {
        var errors = new List<ValidationError>
        {
            new ValidationError("Field1", "Error 1", "ERR_001", ValidationSeverity.Error),
            new ValidationError("Field2", "Error 2", "ERR_002", ValidationSeverity.Warning)
        };
        var result = Result<int>.Invalid(errors);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Equal(2, result.ValidationErrors.Count);
    }

    [Fact]
    public void NotFound_WithoutMessage_ShouldCreateNotFoundResult()
    {
        var result = Result<string>.NotFound();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void NotFound_WithMessages_ShouldCreateNotFoundResultWithErrors()
    {
        var result = Result<string>.NotFound("Resource not found", "ID: 123");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Equal(2, result.Errors.Count());
        Assert.Contains("Resource not found", result.Errors);
    }

    [Fact]
    public void Forbidden_ShouldCreateForbiddenResult()
    {
        var result = Result<int>.Forbidden();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
    }

    [Fact]
    public void Unauthorized_ShouldCreateUnauthorizedResult()
    {
        var result = Result<int>.Unauthorized();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }

    [Fact]
    public void Conflict_WithoutMessages_ShouldCreateConflictResult()
    {
        var result = Result<int>.Conflict();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Conflict_WithMessages_ShouldCreateConflictResultWithErrors()
    {
        var result = Result<int>.Conflict("Version mismatch", "Resource was modified");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void CriticalError_WithMessages_ShouldCreateCriticalErrorResult()
    {
        var result = Result<int>.CriticalError("Database connection failed", "Timeout exceeded");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.CriticalError, result.Status);
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void Unavailable_WithMessages_ShouldCreateUnavailableResult()
    {
        var result = Result<int>.Unavailable("Service temporarily unavailable");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Unavailable, result.Status);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessResult()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ImplicitConversion_ToValue_ShouldReturnValue()
    {
        var result = Result<int>.Success(42);
        int value = result;

        Assert.Equal(42, value);
    }

    [Fact]
    public void ImplicitConversion_FromNonGenericResult_ShouldPreserveStatus()
    {
        var nonGenericResult = Result.Error("Test error");
        Result<int> genericResult = nonGenericResult;

        Assert.False(genericResult.IsSuccess);
        Assert.Equal(ResultStatus.Error, genericResult.Status);
        Assert.Contains("Test error", genericResult.Errors);
    }

    [Fact]
    public void ValueType_ShouldReturnCorrectType()
    {
        var result = Result<string>.Success("test");

        Assert.Equal(typeof(string), result.ValueType);
    }

    [Fact]
    public void GetValue_ShouldReturnValue()
    {
        var result = Result<int>.Success(42);
        var value = result.GetValue();

        Assert.Equal(42, value);
    }

    [Fact]
    public void ToPagedResult_ShouldConvertToPagedResult()
    {
        var result = Result<List<string>>.Success(new List<string> { "Item1", "Item2" });
        var pagedInfo = new PagedInfo(1, 10, 1, 2);

        var pagedResult = result.ToPagedResult(pagedInfo);

        Assert.True(pagedResult.IsSuccess);
        Assert.Equal(1, pagedResult.PagedInfo.PageNumber);
        Assert.Equal(10, pagedResult.PagedInfo.PageSize);
        Assert.Equal(2, pagedResult.Value!.Count);
    }

    [Fact]
    public void ToPagedResult_WithErrorResult_ShouldPreserveErrors()
    {
        var result = Result<List<string>>.Error("Failed to load");
        var pagedInfo = new PagedInfo(1, 10, 0, 0);

        var pagedResult = result.ToPagedResult(pagedInfo);

        Assert.False(pagedResult.IsSuccess);
        Assert.Equal(ResultStatus.Error, pagedResult.Status);
        Assert.Contains("Failed to load", pagedResult.Errors);
    }

    [Fact]
    public void ToPagedResult_ShouldPreserveAllProperties()
    {
        var result = Result<int>.Success(42, "Success message");
        result.GetType().GetProperty("CorrelationId")!.SetValue(result, "correlation-123");
        var pagedInfo = new PagedInfo(1, 10, 1, 1);

        var pagedResult = result.ToPagedResult(pagedInfo);

        Assert.Equal("Success message", pagedResult.SuccessMessage);
        Assert.Equal("correlation-123", pagedResult.CorrelationId);
    }

    [Fact]
    public void IsSuccess_WithOkStatus_ShouldBeTrue()
    {
        var result = Result<int>.Success(1);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void IsSuccess_WithErrorStatus_ShouldBeFalse()
    {
        var result = Result<int>.Error("Error occurred");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Constructor_WithValue_ShouldSetValue()
    {
        var result = new Result<int>(42);

        Assert.Equal(42, result.Value);
        Assert.Equal(ResultStatus.Ok, result.Status);
    }
}
