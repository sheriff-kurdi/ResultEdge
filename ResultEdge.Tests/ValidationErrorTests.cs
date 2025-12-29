using ResultEdge;

namespace ResultEdge.Tests;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_Parameterless_ShouldCreateEmptyValidationError()
    {
        var error = new ValidationError();

        Assert.Null(error.Identifier);
        Assert.Null(error.ErrorMessage);
        Assert.Null(error.ErrorCode);
        Assert.Equal(ValidationSeverity.Error, error.Severity);
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetErrorMessage()
    {
        var error = new ValidationError("Invalid email format");

        Assert.Equal("Invalid email format", error.ErrorMessage);
        Assert.Null(error.Identifier);
        Assert.Null(error.ErrorCode);
        Assert.Equal(ValidationSeverity.Error, error.Severity);
    }

    [Fact]
    public void Constructor_WithAllParameters_ShouldSetAllProperties()
    {
        var error = new ValidationError("Email", "Invalid email format", "EMAIL_001", ValidationSeverity.Warning);

        Assert.Equal("Email", error.Identifier);
        Assert.Equal("Invalid email format", error.ErrorMessage);
        Assert.Equal("EMAIL_001", error.ErrorCode);
        Assert.Equal(ValidationSeverity.Warning, error.Severity);
    }

    [Fact]
    public void DefaultSeverity_ShouldBeError()
    {
        var error1 = new ValidationError();
        var error2 = new ValidationError("Test message");

        Assert.Equal(ValidationSeverity.Error, error1.Severity);
        Assert.Equal(ValidationSeverity.Error, error2.Severity);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var error = new ValidationError
        {
            Identifier = "Username",
            ErrorMessage = "Username is required",
            ErrorCode = "USER_001",
            Severity = ValidationSeverity.Error
        };

        Assert.Equal("Username", error.Identifier);
        Assert.Equal("Username is required", error.ErrorMessage);
        Assert.Equal("USER_001", error.ErrorCode);
        Assert.Equal(ValidationSeverity.Error, error.Severity);
    }

    [Fact]
    public void Severity_CanBeSetToWarning()
    {
        var error = new ValidationError("Field", "Warning message", "WARN_001", ValidationSeverity.Warning);

        Assert.Equal(ValidationSeverity.Warning, error.Severity);
    }

    [Fact]
    public void Severity_CanBeSetToInfo()
    {
        var error = new ValidationError("Field", "Info message", "INFO_001", ValidationSeverity.Info);

        Assert.Equal(ValidationSeverity.Info, error.Severity);
    }

    [Fact]
    public void Constructor_WithEmptyString_ShouldAcceptEmptyString()
    {
        var error = new ValidationError(string.Empty);

        Assert.Equal(string.Empty, error.ErrorMessage);
    }

    [Fact]
    public void Constructor_WithNullValues_ShouldHandleNulls()
    {
        var error = new ValidationError(null!, null!, null!, ValidationSeverity.Error);

        Assert.Null(error.Identifier);
        Assert.Null(error.ErrorMessage);
        Assert.Null(error.ErrorCode);
    }

    [Fact]
    public void MultipleErrors_WithDifferentSeverities_ShouldWork()
    {
        var errors = new List<ValidationError>
        {
            new ValidationError("Field1", "Error", "ERR_001", ValidationSeverity.Error),
            new ValidationError("Field2", "Warning", "WARN_001", ValidationSeverity.Warning),
            new ValidationError("Field3", "Info", "INFO_001", ValidationSeverity.Info)
        };

        Assert.Equal(ValidationSeverity.Error, errors[0].Severity);
        Assert.Equal(ValidationSeverity.Warning, errors[1].Severity);
        Assert.Equal(ValidationSeverity.Info, errors[2].Severity);
    }

    [Fact]
    public void PropertyInitializer_ShouldWork()
    {
        var error = new ValidationError
        {
            Identifier = "Age",
            ErrorMessage = "Age must be between 0 and 120",
            ErrorCode = "AGE_RANGE",
            Severity = ValidationSeverity.Error
        };

        Assert.Equal("Age", error.Identifier);
        Assert.Equal("Age must be between 0 and 120", error.ErrorMessage);
        Assert.Equal("AGE_RANGE", error.ErrorCode);
    }

    [Fact]
    public void ValidationError_WithLongMessage_ShouldAcceptLongStrings()
    {
        var longMessage = new string('x', 1000);
        var error = new ValidationError(longMessage);

        Assert.Equal(1000, error.ErrorMessage!.Length);
    }

    [Fact]
    public void ValidationError_CanBeModifiedAfterCreation()
    {
        var error = new ValidationError("Initial message");

        error.ErrorMessage = "Updated message";
        error.Identifier = "UpdatedField";
        error.ErrorCode = "UPD_001";
        error.Severity = ValidationSeverity.Warning;

        Assert.Equal("Updated message", error.ErrorMessage);
        Assert.Equal("UpdatedField", error.Identifier);
        Assert.Equal("UPD_001", error.ErrorCode);
        Assert.Equal(ValidationSeverity.Warning, error.Severity);
    }
}
