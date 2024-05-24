using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTest
{
    [Theory]
    [InlineData("Some Text", null, "*Some Text*")]
    [InlineData(Strings.Empty, 15, "**")]
    [InlineData(null, -1, "**")]
    public static async Task SearchAsync_InputIsNotNull_ExpectDataverseApiCalledOnce(
        string? sourceSearchString, int? sourceTop, string expectedSearchString)
    {
        var dataverseOut = new DataverseSearchOut(17, SomeDataverseItems);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmContactApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySupplier>(), Mock.Of<ISqlQueryEntitySetSupplier>());

        var input = new ContactSetSearchIn(
            searchText: sourceSearchString,
            customerId: new("2a5d892f-1400-ec11-94ef-000d3a4a099f"))
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
    [InlineData(DataverseFailureCode.Throttling, ContactSetGetFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, ContactSetGetFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, ContactSetGetFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, ContactSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, ContactSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, ContactSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, ContactSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, ContactSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, ContactSetGetFailureCode.Unknown)]
    public static async Task SearchAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, ContactSetGetFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some Failure message");
        var mockDataverseApi = CreateMockDataverseApi(dataverseFailure);

        var api = new CrmContactApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySupplier>(), Mock.Of<ISqlQueryEntitySetSupplier>());

        var actual = await api.SearchAsync(SomeContactSetSearchInput, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some Failure message");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmContactApiTestSource.OutputSearchTestData), MemberType = typeof(CrmContactApiTestSource))]
    public static async Task SearchAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        DataverseSearchOut dataverseSearchOutput, ContactSetSearchOut expected)
    {
        var mockDataverseApi = CreateMockDataverseApi(dataverseSearchOutput);
        var api = new CrmContactApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySupplier>(), Mock.Of<ISqlQueryEntitySetSupplier>());

        var actual = await api.SearchAsync(SomeContactSetSearchInput, CancellationToken.None);

        Assert.StrictEqual(expected, actual);
    }
}