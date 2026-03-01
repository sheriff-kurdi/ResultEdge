using ResultEdge;

namespace ResultEdge.Tests;

public class PagedResultTests
{
    [Fact]
    public void Constructor_ShouldSetDataAndPagedInfo()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);
        var items = new List<string> { "Item1", "Item2", "Item3" };

        var pagedResult = new PagedResult<List<string>>(pagedInfo, items);

        Assert.Equal(items, pagedResult.Data);
        Assert.Equal(pagedInfo, pagedResult.PagedInfo);
    }

    [Fact]
    public void Constructor_ShouldInheritFromResult()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 5);

        var pagedResult = new PagedResult<int>(pagedInfo, 42);

        Assert.IsAssignableFrom<Result<int>>(pagedResult);
    }

    [Fact]
    public void PagedResult_ShouldHaveSuccessStatusByDefault()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 10);
        var items = new List<int> { 1, 2, 3, 4, 5 };

        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        Assert.True(pagedResult.IsSuccess);
        Assert.False(pagedResult.IsFailure);
        Assert.Equal(ResultStatus.Ok, pagedResult.Status);
    }

    [Fact]
    public void PagedInfo_ShouldBeReadOnly()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);
        var items = new List<string> { "Item1" };

        var pagedResult = new PagedResult<List<string>>(pagedInfo, items);

        Assert.Equal(pagedInfo, pagedResult.PagedInfo);
        var property = typeof(PagedResult<List<string>>).GetProperty(nameof(PagedResult<List<string>>.PagedInfo));
        Assert.NotNull(property);
        Assert.True(property!.CanRead);
        Assert.False(property.CanWrite);
    }

    [Fact]
    public void PagedResult_WithEmptyList_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 10, 0, 0);
        var items = new List<string>();

        var pagedResult = new PagedResult<List<string>>(pagedInfo, items);

        Assert.True(pagedResult.IsSuccess);
        Assert.Empty(pagedResult.Data!);
        Assert.Equal(0, pagedResult.PagedInfo!.TotalRecords);
    }

    [Fact]
    public void PagedResult_WithSingleItem_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 1, 1, 1);
        var items = new List<int> { 42 };

        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        Assert.Single(pagedResult.Data!);
        Assert.Equal(42, pagedResult.Data![0]);
    }

    [Fact]
    public void PagedResult_WithComplexObject_ShouldWork()
    {
        var pagedInfo = new PagedInfo(2, 5, 10, 50);
        var items = new List<ComplexObject>
        {
            new ComplexObject { Id = 1, Name = "Object1" },
            new ComplexObject { Id = 2, Name = "Object2" }
        };

        var pagedResult = new PagedResult<List<ComplexObject>>(pagedInfo, items);

        Assert.Equal(2, pagedResult.Data!.Count);
        Assert.Equal("Object1", pagedResult.Data[0].Name);
        Assert.Equal(2, pagedResult.PagedInfo!.PageNumber);
    }

    [Fact]
    public void PagedResult_InheritsResultProperties_ShouldAccessBaseProperties()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 5);
        var items = new List<string> { "Test" };

        var pagedResult = new PagedResult<List<string>>(pagedInfo, items);

        Assert.Empty(pagedResult.Errors);
        Assert.Empty(pagedResult.ValidationErrors);
        Assert.Equal(string.Empty, pagedResult.SuccessMessage);
        Assert.Equal(string.Empty, pagedResult.CorrelationId);
    }

    [Fact]
    public void ToPagedResult_FromResult_ShouldCreatePagedResult()
    {
        var items = new List<int> { 1, 2, 3, 4, 5 };
        var result = Result<List<int>>.Success(items);
        var pagedInfo = new PagedInfo(1, 5, 1, 5);

        var pagedResult = result.ToPagedResult(pagedInfo);

        Assert.IsType<PagedResult<List<int>>>(pagedResult);
        Assert.Equal(5, pagedResult.Data!.Count);
        Assert.Equal(pagedInfo.PageNumber, pagedResult.PagedInfo!.PageNumber);
    }

    [Fact]
    public void PagedResult_WithLargePageNumber_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1000, 50, 2000, 100000);
        var items = Enumerable.Range(1, 50).ToList();

        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        Assert.Equal(1000, pagedResult.PagedInfo!.PageNumber);
        Assert.Equal(50, pagedResult.Data!.Count);
    }

    [Fact]
    public void PagedResult_WithStringData_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 1, 1, 1);

        var pagedResult = new PagedResult<string>(pagedInfo, "Single string value");

        Assert.Equal("Single string value", pagedResult.Data);
        Assert.True(pagedResult.IsSuccess);
    }

    [Fact]
    public void PagedResult_WithPrimitiveType_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 1, 1, 1);

        var pagedResult = new PagedResult<int>(pagedInfo, 42);

        Assert.Equal(42, pagedResult.Data);
        Assert.Equal(typeof(int), pagedResult.DataType);
    }

    [Fact]
    public void PagedResult_PagedInfo_ReflectsPagedInfoMutation()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);
        var items = new List<string> { "Item1" };
        var pagedResult = new PagedResult<List<string>>(pagedInfo, items);

        pagedInfo.SetPageNumber(2).SetTotalRecords(100);

        Assert.Equal(2, pagedResult.PagedInfo!.PageNumber);
        Assert.Equal(100, pagedResult.PagedInfo!.TotalRecords);
    }

    [Fact]
    public void PagedResult_WithArray_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 3, 1, 3);
        var items = new[] { "A", "B", "C" };

        var pagedResult = new PagedResult<string[]>(pagedInfo, items);

        Assert.Equal(3, pagedResult.Data!.Length);
        Assert.Equal("B", pagedResult.Data[1]);
    }

    [Fact]
    public void PagedResult_GetData_ShouldReturnData()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 10);
        var items = new List<int> { 1, 2, 3 };
        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        var data = pagedResult.GetData();

        Assert.IsType<List<int>>(data);
        Assert.Equal(3, ((List<int>)data!).Count);
    }

    // ── Error factory methods ────────────────────────────────────────────────

    [Fact]
    public void Error_ShouldSetStatusAndNullPagedInfo()
    {
        var result = PagedResult<List<int>>.Error("Something went wrong");

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Null(result.PagedInfo);
        Assert.Contains("Something went wrong", result.Errors);
    }

    [Fact]
    public void ErrorWithCorrelationId_ShouldSetCorrelationIdAndNullPagedInfo()
    {
        var result = PagedResult<List<int>>.ErrorWithCorrelationId("corr-123", "Failure");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Equal("corr-123", result.CorrelationId);
        Assert.Null(result.PagedInfo);
    }

    [Fact]
    public void NotFound_ShouldSetStatusAndNullPagedInfo()
    {
        var result = PagedResult<List<int>>.NotFound("No items found");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Null(result.PagedInfo);
    }

    [Fact]
    public void Success_StaticFactory_ShouldCreateSuccessPagedResult()
    {
        var pagedInfo = new PagedInfo(1, 10, 3, 25);
        var items = new List<int> { 1, 2, 3 };

        var result = PagedResult<List<int>>.Success(pagedInfo, items);

        Assert.True(result.IsSuccess);
        Assert.Equal(pagedInfo, result.PagedInfo);
        Assert.Equal(items, result.Data);
    }

    [Fact]
    public void Success_StaticFactory_WithMessage_ShouldSetSuccessMessage()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 3);
        var items = new List<string> { "a", "b", "c" };

        var result = PagedResult<List<string>>.Success(pagedInfo, items, "Loaded successfully");

        Assert.True(result.IsSuccess);
        Assert.Equal("Loaded successfully", result.SuccessMessage);
        Assert.Equal(pagedInfo, result.PagedInfo);
    }

    private class ComplexObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
