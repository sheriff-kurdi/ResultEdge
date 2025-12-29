using ResultEdge;

namespace ResultEdge.Tests;

public class ResultVoidTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Empty(result.Errors);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public void SuccessWithMessage_ShouldSetSuccessMessage()
    {
        var result = Result.SuccessWithMessage("Operation completed");

        Assert.True(result.IsSuccess);
        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal("Operation completed", result.SuccessMessage);
    }

    [Fact]
    public void Success_Generic_ShouldCreateGenericResult()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.IsType<Result<int>>(result);
    }

    [Fact]
    public void Success_GenericWithMessage_ShouldSetSuccessMessage()
    {
        var result = Result.Success("test", "Success message");

        Assert.True(result.IsSuccess);
        Assert.Equal("test", result.Value);
        Assert.Equal("Success message", result.SuccessMessage);
    }

    [Fact]
    public void Error_WithMessages_ShouldCreateErrorResult()
    {
        var result = Result.Error("Error 1", "Error 2");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Equal(2, result.Errors.Count());
        Assert.Contains("Error 1", result.Errors);
        Assert.Contains("Error 2", result.Errors);
    }

    [Fact]
    public void ErrorWithCorrelationId_ShouldSetCorrelationId()
    {
        var result = Result.ErrorWithCorrelationId("correlation-123", "Error occurred");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Equal("correlation-123", result.CorrelationId);
        Assert.Contains("Error occurred", result.Errors);
    }

    [Fact]
    public void ErrorWithCorrelationId_WithMultipleMessages_ShouldSetAllMessages()
    {
        var result = Result.ErrorWithCorrelationId("corr-456", "Error 1", "Error 2", "Error 3");

        Assert.Equal("corr-456", result.CorrelationId);
        Assert.Equal(3, result.Errors.Count());
    }

    [Fact]
    public void Invalid_WithSingleValidationError_ShouldCreateInvalidResult()
    {
        var validationError = new ValidationError("Field1", "Invalid value", "VAL_001", ValidationSeverity.Error);
        var result = Result.Invalid(validationError);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Single(result.ValidationErrors);
        Assert.Equal("Field1", result.ValidationErrors[0].Identifier);
    }

    [Fact]
    public void Invalid_WithMultipleValidationErrors_ShouldCreateInvalidResult()
    {
        var errors = new[]
        {
            new ValidationError("Email", "Invalid email", "EMAIL_001", ValidationSeverity.Error),
            new ValidationError("Phone", "Invalid phone", "PHONE_001", ValidationSeverity.Error)
        };
        var result = Result.Invalid(errors);

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
            new ValidationError("Field2", "Error 2", "ERR_002", ValidationSeverity.Warning),
            new ValidationError("Field3", "Error 3", "ERR_003", ValidationSeverity.Info)
        };
        var result = Result.Invalid(errors);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Equal(3, result.ValidationErrors.Count);
    }

    [Fact]
    public void NotFound_WithoutMessage_ShouldCreateNotFoundResult()
    {
        var result = Result.NotFound();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void NotFound_WithMessages_ShouldCreateNotFoundResultWithErrors()
    {
        var result = Result.NotFound("Resource not found", "ID: 123");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Equal(2, result.Errors.Count());
        Assert.Contains("Resource not found", result.Errors);
        Assert.Contains("ID: 123", result.Errors);
    }

    [Fact]
    public void Forbidden_ShouldCreateForbiddenResult()
    {
        var result = Result.Forbidden();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
    }

    [Fact]
    public void Unauthorized_ShouldCreateUnauthorizedResult()
    {
        var result = Result.Unauthorized();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }

    [Fact]
    public void Conflict_WithoutMessages_ShouldCreateConflictResult()
    {
        var result = Result.Conflict();

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Conflict_WithMessages_ShouldCreateConflictResultWithErrors()
    {
        var result = Result.Conflict("Version mismatch", "Resource was modified");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(2, result.Errors.Count());
        Assert.Contains("Version mismatch", result.Errors);
    }

    [Fact]
    public void CriticalError_WithMessages_ShouldCreateCriticalErrorResult()
    {
        var result = Result.CriticalError("Database failure", "Connection lost");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.CriticalError, result.Status);
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void Unavailable_WithMessages_ShouldCreateUnavailableResult()
    {
        var result = Result.Unavailable("Service temporarily unavailable", "Please retry");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Unavailable, result.Status);
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void Result_InheritsFromResultOfResult()
    {
        var result = Result.Success();

        Assert.IsAssignableFrom<Result<Result>>(result);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateSuccessResult()
    {
        var result = new Result();

        Assert.True(result.IsSuccess);
        Assert.Equal(ResultStatus.Ok, result.Status);
    }
}
