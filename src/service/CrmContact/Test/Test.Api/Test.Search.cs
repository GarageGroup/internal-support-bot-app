using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTest
{
    [Fact]
    public static async Task SearchAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseSearchOut(1, SomeDataverseItems);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmContactApi(mockDataverseApi.Object);

        var cancellationToken = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);

        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.SearchAsync(null!, cancellationToken);
    }

    [Theory]
    [InlineData("Some Text", null, "*Some Text*")]
    [InlineData(Strings.Empty, 15, "**")]
    [InlineData(null, -1, "**")]
    public static async Task SearchAsync_InputIsNotNull_ExpectDataverseApiCalledOnce(
        string? sourceSearchString, int? sourceTop, string expectedSearchString)
    {
        var dataverseOut = new DataverseSearchOut(17, SomeDataverseItems);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmContactApi(mockDataverseApi.Object);

        var input = new ContactSetSearchIn(
            searchText: sourceSearchString,
            customerId: Guid.Parse("2a5d892f-1400-ec11-94ef-000d3a4a099f"))
        {
            Top = sourceTop
        };

        var cancellationToken = new CancellationToken(canceled: false);

        _ = await api.SearchAsync(input, cancellationToken);

        var expected = new DataverseSearchIn(expectedSearchString)
        {
            Entities = new("contact"),
            Filter = "parentcustomerid eq '2a5d892f-1400-ec11-94ef-000d3a4a099f'",
            Top = sourceTop
        };

        mockDataverseApi.Verify(a => a.SearchAsync(expected, cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Throttling, ContactSetSearchFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, ContactSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, ContactSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, ContactSetSearchFailureCode.Unknown)]
    public static async Task SearchAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, ContactSetSearchFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some Failure message");
        var mockDataverseApi = CreateMockDataverseApi(dataverseFailure);

        var api = new CrmContactApi(mockDataverseApi.Object);

        var actual = await api.SearchAsync(SomeContactSetSearchInput, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some Failure message");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmContactApiTestSource.OutputTestData), MemberType = typeof(CrmContactApiTestSource))]
    public static async Task SearchAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        DataverseSearchOut dataverseSearchOutput, ContactSetSearchOut expected)
    {
        var mockDataverseApi = CreateMockDataverseApi(dataverseSearchOutput);
        var api = new CrmContactApi(mockDataverseApi.Object);

        var actual = await api.SearchAsync(SomeContactSetSearchInput, CancellationToken.None);

        Assert.StrictEqual(expected, actual);
    }
}