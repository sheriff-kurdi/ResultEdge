using ResultEdge;

namespace ResultEdge.Tests;

public class PagedInfoTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        Assert.Equal(1, pagedInfo.PageNumber);
        Assert.Equal(10, pagedInfo.PageSize);
        Assert.Equal(5, pagedInfo.TotalPages);
        Assert.Equal(50, pagedInfo.TotalRecords);
    }

    [Fact]
    public void Constructor_WithZeroValues_ShouldAcceptZeroValues()
    {
        var pagedInfo = new PagedInfo(0, 0, 0, 0);

        Assert.Equal(0, pagedInfo.PageNumber);
        Assert.Equal(0, pagedInfo.PageSize);
        Assert.Equal(0, pagedInfo.TotalPages);
        Assert.Equal(0, pagedInfo.TotalRecords);
    }

    [Fact]
    public void Constructor_WithLargeValues_ShouldHandleLargeNumbers()
    {
        var pagedInfo = new PagedInfo(1000, 100, 10000, 1000000);

        Assert.Equal(1000, pagedInfo.PageNumber);
        Assert.Equal(100, pagedInfo.PageSize);
        Assert.Equal(10000, pagedInfo.TotalPages);
        Assert.Equal(1000000, pagedInfo.TotalRecords);
    }

    [Fact]
    public void SetPageNumber_ShouldUpdatePageNumber()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        var result = pagedInfo.SetPageNumber(2);

        Assert.Equal(2, pagedInfo.PageNumber);
        Assert.Same(pagedInfo, result);
    }

    [Fact]
    public void SetPageSize_ShouldUpdatePageSize()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        var result = pagedInfo.SetPageSize(20);

        Assert.Equal(20, pagedInfo.PageSize);
        Assert.Same(pagedInfo, result);
    }

    [Fact]
    public void SetTotalPages_ShouldUpdateTotalPages()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        var result = pagedInfo.SetTotalPages(10);

        Assert.Equal(10, pagedInfo.TotalPages);
        Assert.Same(pagedInfo, result);
    }

    [Fact]
    public void SetTotalRecords_ShouldUpdateTotalRecords()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        var result = pagedInfo.SetTotalRecords(100);

        Assert.Equal(100, pagedInfo.TotalRecords);
        Assert.Same(pagedInfo, result);
    }

    [Fact]
    public void FluentAPI_ShouldAllowChaining()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50)
            .SetPageNumber(3)
            .SetPageSize(25)
            .SetTotalPages(8)
            .SetTotalRecords(200);

        Assert.Equal(3, pagedInfo.PageNumber);
        Assert.Equal(25, pagedInfo.PageSize);
        Assert.Equal(8, pagedInfo.TotalPages);
        Assert.Equal(200, pagedInfo.TotalRecords);
    }

    [Fact]
    public void FluentAPI_MultipleSetters_ShouldReturnSameInstance()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        var result1 = pagedInfo.SetPageNumber(2);
        var result2 = result1.SetPageSize(20);
        var result3 = result2.SetTotalPages(10);
        var result4 = result3.SetTotalRecords(100);

        Assert.Same(pagedInfo, result1);
        Assert.Same(pagedInfo, result2);
        Assert.Same(pagedInfo, result3);
        Assert.Same(pagedInfo, result4);
    }

    [Fact]
    public void SetPageNumber_WithZero_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        pagedInfo.SetPageNumber(0);

        Assert.Equal(0, pagedInfo.PageNumber);
    }

    [Fact]
    public void SetPageSize_WithZero_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        pagedInfo.SetPageSize(0);

        Assert.Equal(0, pagedInfo.PageSize);
    }

    [Fact]
    public void Properties_WithPrivateSet_ShouldNotBeModifiableDirectly()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);
        var pageNumberProperty = typeof(PagedInfo).GetProperty(nameof(PagedInfo.PageNumber));
        var pageSizeProperty = typeof(PagedInfo).GetProperty(nameof(PagedInfo.PageSize));
        var totalPagesProperty = typeof(PagedInfo).GetProperty(nameof(PagedInfo.TotalPages));
        var totalRecordsProperty = typeof(PagedInfo).GetProperty(nameof(PagedInfo.TotalRecords));

        Assert.False(pageNumberProperty!.CanWrite && pageNumberProperty.SetMethod!.IsPublic);
        Assert.False(pageSizeProperty!.CanWrite && pageSizeProperty.SetMethod!.IsPublic);
        Assert.False(totalPagesProperty!.CanWrite && totalPagesProperty.SetMethod!.IsPublic);
        Assert.False(totalRecordsProperty!.CanWrite && totalRecordsProperty.SetMethod!.IsPublic);
    }

    [Fact]
    public void SetMethods_ShouldOnlyWayToModifyProperties()
    {
        var pagedInfo = new PagedInfo(1, 10, 5, 50);

        pagedInfo.SetPageNumber(99);

        Assert.Equal(99, pagedInfo.PageNumber);
    }

    [Fact]
    public void PagedInfo_ForEmptyResults_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 10, 0, 0);

        Assert.Equal(1, pagedInfo.PageNumber);
        Assert.Equal(10, pagedInfo.PageSize);
        Assert.Equal(0, pagedInfo.TotalPages);
        Assert.Equal(0, pagedInfo.TotalRecords);
    }

    [Fact]
    public void PagedInfo_ForSinglePage_ShouldWork()
    {
        var pagedInfo = new PagedInfo(1, 10, 1, 5);

        Assert.Equal(1, pagedInfo.PageNumber);
        Assert.Equal(1, pagedInfo.TotalPages);
        Assert.Equal(5, pagedInfo.TotalRecords);
    }

    [Fact]
    public void PagedInfo_ForLargeDataset_ShouldWork()
    {
        var pagedInfo = new PagedInfo(500, 50, 2000, 100000);

        Assert.Equal(500, pagedInfo.PageNumber);
        Assert.Equal(50, pagedInfo.PageSize);
        Assert.Equal(2000, pagedInfo.TotalPages);
        Assert.Equal(100000, pagedInfo.TotalRecords);
    }
}
