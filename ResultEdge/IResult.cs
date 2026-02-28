namespace ResultEdge;

public interface IResult
{
    ResultStatus Status { get; }
    IEnumerable<string> Errors { get; }
    List<ValidationError> ValidationErrors { get; }
    Type DataType { get; }
    object GetData();
}
