using ResultEdge;

namespace ResultEdge.Tests;

public class PagedResultTests
{
    [Fact]
    public void Constructor_ShouldSetValueAndPagedInfo()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);
        var items = new List<string> { "Item1", "Item2", "Item3" };

        var pagedResult = new PagedResult<List<string>>(pagedInfo, items);

        Assert.Equal(items, pagedResult.Value);
        Assert.Equal(pagedInfo, pagedResult.PagedInfo);
    }

    [Fact]
    public void Constructor_ShouldInheritFromResult()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 5);
        var value = 42;

        var pagedResult = new PagedResult<int>(pagedInfo, value);

        Assert.IsAssignableFrom<Result<int>>(pagedResult);
    }

    [Fact]
    public void PagedResult_ShouldHaveSuccessStatusByDefault()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 10);
        var items = new List<int> { 1, 2, 3, 4, 5 };

        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        Assert.True(pagedResult.IsSuccess);
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
        Assert.Empty(pagedResult.Value!);
        Assert.Equal(0, pagedResult.PagedInfo.TotalRecords);
    }

    [Fact]
    public void PagedResult_WithSingleItem_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 1, 1, 1);
        var items = new List<int> { 42 };

        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        Assert.Single(pagedResult.Value!);
        Assert.Equal(42, pagedResult.Value[0]);
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

        Assert.Equal(2, pagedResult.Value!.Count);
        Assert.Equal("Object1", pagedResult.Value[0].Name);
        Assert.Equal(2, pagedResult.PagedInfo.PageNumber);
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
        Assert.Equal(5, pagedResult.Value!.Count);
        Assert.Equal(pagedInfo.PageNumber, pagedResult.PagedInfo.PageNumber);
    }

    [Fact]
    public void PagedResult_WithLargePageNumber_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1000, 50, 2000, 100000);
        var items = Enumerable.Range(1, 50).ToList();

        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        Assert.Equal(1000, pagedResult.PagedInfo.PageNumber);
        Assert.Equal(50, pagedResult.Value!.Count);
    }

    [Fact]
    public void PagedResult_WithStringValue_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 1, 1, 1);
        var value = "Single string value";

        var pagedResult = new PagedResult<string>(pagedInfo, value);

        Assert.Equal("Single string value", pagedResult.Value);
        Assert.True(pagedResult.IsSuccess);
    }

    [Fact]
    public void PagedResult_WithPrimitiveType_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 1, 1, 1);

        var pagedResult = new PagedResult<int>(pagedInfo, 42);

        Assert.Equal(42, pagedResult.Value);
        Assert.Equal(typeof(int), pagedResult.ValueType);
    }

    [Fact]
    public void PagedResult_PagedInfo_CanBeModifiedViaSetters()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);
        var items = new List<string> { "Item1" };
        var pagedResult = new PagedResult<List<string>>(pagedInfo, items);

        pagedInfo.SetPageNumber(2).SetTotalRecords(100);

        Assert.Equal(2, pagedResult.PagedInfo.PageNumber);
        Assert.Equal(100, pagedResult.PagedInfo.TotalRecords);
    }

    [Fact]
    public void PagedResult_WithArray_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 3, 1, 3);
        var items = new[] { "A", "B", "C" };

        var pagedResult = new PagedResult<string[]>(pagedInfo, items);

        Assert.Equal(3, pagedResult.Value!.Length);
        Assert.Equal("B", pagedResult.Value[1]);
    }

    [Fact]
    public void PagedResult_GetValue_ShouldReturnValue()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 10);
        var items = new List<int> { 1, 2, 3 };
        var pagedResult = new PagedResult<List<int>>(pagedInfo, items);

        var value = pagedResult.GetValue();

        Assert.IsType<List<int>>(value);
        Assert.Equal(3, ((List<int>)value).Count);
    }

    private class ComplexObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
