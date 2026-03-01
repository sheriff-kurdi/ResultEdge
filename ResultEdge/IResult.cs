namespace ResultEdge;

public interface IResult
{
    ResultStatus Status { get; }
    bool IsSuccess { get; }
    bool IsFailure { get; }
    string SuccessMessage { get; }
    string CorrelationId { get; }
    IEnumerable<string> Errors { get; }
    IReadOnlyList<ValidationError> ValidationErrors { get; }
    Type DataType { get; }
    object? GetData();
}
