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
    public static async Task GetAsync_ExpectSqlApiCalledOnce()
    {
        var mockSqlApi = BuildMockSqlEntityApi(SomeDbIncident);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        const long telegramSenderId = 179190124307;
        var input = new ContactGetIn(telegramSenderId);

        _ = await api.GetAsync(input, TestContext.Current.CancellationToken);

        var expectedQuery = new DbSelectQuery("incident", "i")
        {
            Top = 1,
            SelectedFields =
            [
                "i.customerid AS CustomerId",
                "i.customeridname AS CustomerName",
                "i.primarycontactid AS ContactId",
                "i.primarycontactidname AS ContactName"
            ],
            Filter = new DbParameterFilter(
                fieldName: "i.gg_sender_telegram_id",
                @operator: DbFilterOperator.Equal,
                fieldValue: telegramSenderId,
                parameterName: "SenderTelegramId"),
            Orders =
            [
                new("i.createdon", DbOrderType.Descending)
            ]
        };

        mockSqlApi.Verify(a => a.QueryEntityOrFailureAsync<DbIncident>(expectedQuery, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(EntityQueryFailureCode.NotFound, ContactGetFailureCode.NotFound)]
    [InlineData(EntityQueryFailureCode.Unknown, ContactGetFailureCode.Unknown)]
    public static async Task GetAsync_DbResultIsFailure_ExpectFailure(
        EntityQueryFailureCode sourceFailureCode, ContactGetFailureCode expectedFailureCode)
    {
        var sourceException = new Exception("Some exception message");
        var dbFailure = sourceException.ToFailure(sourceFailureCode, "Some text");

        var mockSqlApi = BuildMockSqlEntityApi(dbFailure);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var actual = await api.GetAsync(SomeContactGetInput, TestContext.Current.CancellationToken);
        var expected = Failure.Create(expectedFailureCode, "Some text", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmContactApiTestSource.OutputGetTestData), MemberType = typeof(CrmContactApiTestSource))]
    internal static async Task GetAsync_DataverseGetResultIsSuccess_ExpectSuccess(
        DbIncident dbIncident, ContactGetOut expected)
    {
        var mockSqlApi = BuildMockSqlEntityApi(dbIncident);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object, Mock.Of<ISqlQueryEntitySetSupplier>());

        var actual = await api.GetAsync(SomeContactGetInput, TestContext.Current.CancellationToken);

        Assert.StrictEqual(expected, actual);
    }
}