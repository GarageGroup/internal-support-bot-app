using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

partial class CrmCustomerApiTest
{
    [Fact]
    public static async Task SearchAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseSearchOut(1, SomeDataverseItems);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmCustomerApi(mockDataverseApi.Object);

        var cancellationToken = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);

        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.SearchAsync(null!, cancellationToken);
    }

    [Theory]
    [InlineData("Some text", null, "*Some text*")]
    [InlineData(Strings.Empty, 15, "**")]
    [InlineData(null, -1, "**")]
    public static async Task SearchAsync_InputIsNotNull_ExpectDataverseApiCalledOnce(
        string? sourceSearchString, int? sourceTop, string expectedSearchString)
    {
        var dataverseOut = new DataverseSearchOut(17, SomeDataverseItems);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmCustomerApi(mockDataverseApi.Object);

        var input = new CustomerSetSearchIn(sourceSearchString)
        {
            Top = sourceTop
        };

        var cancellationToken = new CancellationToken(canceled: false);

        _ = await api.SearchAsync(input, cancellationToken);

        var expected = new DataverseSearchIn(expectedSearchString)
        {
            Entities = new("account"),
            Top = sourceTop
        };

        mockDataverseApi.Verify(a => a.SearchAsync(expected, cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Throttling, CustomerSetSearchFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, CustomerSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, CustomerSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, CustomerSetSearchFailureCode.Unknown)]
    public static async Task SearchAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, CustomerSetSearchFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some failure message");
        var mockDataverseApi = CreateMockDataverseApi(dataverseFailure);

        var api = new CrmCustomerApi(mockDataverseApi.Object);

        var input = new CustomerSetSearchIn("Some search text")
        {
            Top = 5
        };

        var actual = await api.SearchAsync(input, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some failure message");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmCustomerApiTestSource.OutputTestData), MemberType = typeof(CrmCustomerApiTestSource))]
    public static async Task SearchAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        DataverseSearchOut dataverseSearchOutput, CustomerSetSearchOut expected)
    {
        var mockDataverseApi = CreateMockDataverseApi(dataverseSearchOutput);
        var api = new CrmCustomerApi(mockDataverseApi.Object);

        var input = new CustomerSetSearchIn("Some Search Text");
        var actual = await api.SearchAsync(input, CancellationToken.None);

        Assert.StrictEqual(expected, actual);
    }
}