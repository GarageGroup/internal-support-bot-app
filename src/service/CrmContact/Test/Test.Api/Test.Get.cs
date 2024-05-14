using GarageGroup.Infra;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTest
{
    [Theory]
    [MemberData(nameof(CrmContactApiTestSource.InputGetInvalidTestData), MemberType = typeof(CrmContactApiTestSource))]
    public static async Task GetAsync_InputInvalid_ExpectInvalidInputFailure(ContactGetIn input)
    {
        var mockSqlApi = BuildMockIncidentSqlApi(SomeDbIncident);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetAsync(input, default);
        var expected = Failure.Create(ContactGetFailureCode.InvalidInput, "Telegram sender id is empty");

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task GetAsync_InputIsValid_ExpectSqlApiCalledOnce()
    {
        var mockSqlApi = BuildMockIncidentSqlApi(SomeDbIncident);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var input = new ContactGetIn(
            telegramSenderId: "123456");

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.GetAsync(input, cancellationToken);

        var expectedQuery = new DbSelectQuery("incident", "i")
        {
            Top = 1,
            SelectedFields = new(
                "i.customerid AS CustomerId",
                "i.customeridname AS CustomerName",
                "i.primarycontactid AS ContactId",
                "i.primarycontactidname AS ContactName"),
            Filter = new DbParameterFilter(
                fieldName: "i.gg_sender_telegram_id",
                @operator: DbFilterOperator.Equal,
                fieldValue: "123456",
                parameterName: "SenderTelegramId"),
            Orders =
            [
                new("i.createdon", DbOrderType.Descending)
            ]
        };

        mockSqlApi.Verify(a => a.QueryEntitySetOrFailureAsync<DbIncident>(expectedQuery, cancellationToken), Times.Once);
    }

    [Fact]
    public static async Task GetAsync_DbResultIsFailure_ExpectUnknownFailure()
    {
        var sourceException = new Exception("Some exception message");
        var dbFailure = sourceException.ToFailure("Some text");

        var mockSqlApi = BuildMockIncidentSqlApi(dbFailure);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetAsync(SomeContactGetInput, default);
        var expected = Failure.Create(ContactGetFailureCode.Unknown, "Some text", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    internal static async Task GetAsync_DataverseGetResultIsEmpty_ExpectNotFoundFailure()
    {
        var output = new FlatArray<DbIncident>();
        var mockSqlApi = BuildMockIncidentSqlApi(output);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetAsync(SomeContactGetInput, default);
        var expected = Failure.Create(ContactGetFailureCode.NotFound, "Incident not found");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmContactApiTestSource.OutputGetTestData), MemberType = typeof(CrmContactApiTestSource))]
    internal static async Task GetAsync_DataverseGetResultIsSuccess_ExpectSuccess(
        FlatArray<DbIncident> dbIncident, ContactGetOut expected)
    {
        var mockSqlApi = BuildMockIncidentSqlApi(dbIncident);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetAsync(SomeContactGetInput, default);

        Assert.StrictEqual(expected, actual);
    }
}