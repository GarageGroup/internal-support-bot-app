using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

partial class CrmCustomerApiTest
{
    [Theory]
    [InlineData("Some text", null, "*Some text*")]
    [InlineData(Strings.Empty, 15, "**")]
    [InlineData(null, -1, "**")]
    public static async Task SearchAsync_InputIsNotNull_ExpectDataverseApiCalledOnce(
        string? sourceSearchString, int? sourceTop, string expectedSearchString)
    {
        var dataverseOut = new DataverseSearchOut(17, SomeDataverseItems);
        var mockDataverseApi = BuildMockDataverseApi(dataverseOut);

        var api = new CrmCustomerApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

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
    [InlineData(DataverseFailureCode.Throttling, CustomerSetGetFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, CustomerSetGetFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, CustomerSetGetFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, CustomerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, CustomerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, CustomerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, CustomerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, CustomerSetGetFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, CustomerSetGetFailureCode.Unknown)]
    public static async Task SearchAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, CustomerSetGetFailureCode expectedFailureCode)
    {
        var sourceException = new Exception("Some error text");
        var dataverseFailure = sourceException.ToFailure(sourceFailureCode, "Some failure message");

        var mockDataverseApi = BuildMockDataverseApi(dataverseFailure);

        var api = new CrmCustomerApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var input = new CustomerSetSearchIn("Some search text")
        {
            Top = 5
        };

        var actual = await api.SearchAsync(input, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some failure message", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmCustomerApiTestSource.OutputSearchTestData), MemberType = typeof(CrmCustomerApiTestSource))]
    public static async Task SearchAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        DataverseSearchOut dataverseSearchOutput, CustomerSetSearchOut expected)
    {
        var mockDataverseApi = BuildMockDataverseApi(dataverseSearchOutput);
        var api = new CrmCustomerApi(mockDataverseApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var input = new CustomerSetSearchIn("Some Search Text");
        var actual = await api.SearchAsync(input, CancellationToken.None);

        Assert.StrictEqual(expected, actual);
    }
}