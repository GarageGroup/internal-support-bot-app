using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmOwner.Test;

partial class CrmOwnerApiTest
{
    [Fact]
    public static async Task SearchAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseSearchOut(1, SomeDataverseItems);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmOwnerApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var cancellationToken = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);

        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.SearchAsync(null!, cancellationToken);
    }

    [Theory]
    [InlineData("Some text", null, "*Some text*")]
    [InlineData(Strings.Empty, 3, "**")]
    [InlineData(null, -1, "**")]
    public static async Task SearchAsync_InputIsNotNull_ExpectDataverseApiCalledOnce(
        string? sourceSearchString, int? sourceTop, string expectedSearchString)
    {
        var dataverseOut = new DataverseSearchOut(17, SomeDataverseItems);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmOwnerApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var input = new OwnerSetSearchIn(sourceSearchString)
        {
            Top = sourceTop
        };

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.SearchAsync(input, cancellationToken);

        var expected = new DataverseSearchIn(expectedSearchString)
        {
            Entities = new("systemuser"),
            Top = sourceTop
        };

        mockDataverseApi.Verify(a => a.SearchAsync(expected, cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Throttling, OwnerSetGetFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, OwnerSetGetFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, OwnerSetGetFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, OwnerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, OwnerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, OwnerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, OwnerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, OwnerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, OwnerSetGetFailureCode.Unknown)]
    public static async Task SearchAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, OwnerSetGetFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some error message");
        var mockDataverseApi = CreateMockDataverseApi(dataverseFailure);

        var api = new CrmOwnerApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var input = new OwnerSetSearchIn("Some search text")
        {
            Top = 5
        };

        var actual = await api.SearchAsync(input, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some error message");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmOwnerApiTestSource.OutputSearchTestData), MemberType = typeof(CrmOwnerApiTestSource))]
    public static async Task SearchAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        DataverseSearchOut dataverseSearchOutput, OwnerSetSearchOut expected)
    {
        var mockDataverseApi = CreateMockDataverseApi(dataverseSearchOutput);
        var api = new CrmOwnerApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var input = new OwnerSetSearchIn("Some Search Text")
        {
            Top = 5
        };

        var actual = await api.SearchAsync(input, CancellationToken.None);

        Assert.StrictEqual(expected, actual);
    }
}