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

        var api = new CrmOwnerApi(mockDataverseApi.Object);

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

        var api = new CrmOwnerApi(mockDataverseApi.Object);

        var input = new UserSetSearchIn(sourceSearchString)
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
    [InlineData(DataverseFailureCode.Throttling, UserSetSearchFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, UserSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, UserSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, UserSetSearchFailureCode.Unknown)]
    public static async Task SearchAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, UserSetSearchFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some error message");
        var mockDataverseApi = CreateMockDataverseApi(dataverseFailure);

        var api = new CrmOwnerApi(mockDataverseApi.Object);

        var input = new UserSetSearchIn("Some search text")
        {
            Top = 5
        };

        var actual = await api.SearchAsync(input, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some error message");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmOwnerApiTestSource.OutputTestData), MemberType = typeof(CrmOwnerApiTestSource))]
    public static async Task SearchAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        DataverseSearchOut dataverseSearchOutput, UserSetSearchOut expected)
    {
        var mockDataverseApi = CreateMockDataverseApi(dataverseSearchOutput);
        var api = new CrmOwnerApi(mockDataverseApi.Object);

        var input = new UserSetSearchIn("Some Search Text")
        {
            Top = 5
        };

        var actual = await api.SearchAsync(input, CancellationToken.None);

        Assert.StrictEqual(expected, actual);
    }
}