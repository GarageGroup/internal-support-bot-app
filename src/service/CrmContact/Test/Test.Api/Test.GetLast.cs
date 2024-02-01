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
    public static async Task GetLastAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockSqlApi = BuildMockSqlApi(SomeDbContacts);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var cancellationToken = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);

        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.GetLastAsync(null!, cancellationToken);
    }

    [Fact]
    public static async Task GetLastAsync_InputIsNotNull_ExpectSqlApiCalledOnce()
    {
        var mockSqlApi = BuildMockSqlApi(SomeDbContacts);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var input = new LastContactSetGetIn(
            customerId: new("24cae7f6-ecf5-40f9-aba5-4fb122141e85"),
            userId: new("15d86420-b8c1-4e9e-b652-2e7195447a3a"),
            top: 15);

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.GetLastAsync(input, cancellationToken);

        var expectedQuery = new DbSelectQuery("contact", "c")
        {
            Top = 15,
            SelectedFields =
            [
                "c.contactid AS Id",
                "c.fullname AS Name",
                "(SELECT MAX(i.createdon) FROM incident i WHERE i.primarycontactid = c.contactid" +
                    " AND i.createdby = '15d86420-b8c1-4e9e-b652-2e7195447a3a') AS LastCurrentUserIncidentDate",
                "(SELECT MAX(i.createdon) FROM incident i WHERE i.primarycontactid = c.contactid) AS LastIncidentDate"
            ],
            Filter = new DbParameterFilter(
                fieldName: "c.parentcustomerid",
                @operator: DbFilterOperator.Equal,
                fieldValue: Guid.Parse("24cae7f6-ecf5-40f9-aba5-4fb122141e85"),
                parameterName: "customerId"),
            Orders =
            [
                new("LastCurrentUserIncidentDate", DbOrderType.Descending),
                new("LastIncidentDate", DbOrderType.Descending),
                new("c.createdon", DbOrderType.Descending)
            ]
        };

        mockSqlApi.Verify(a => a.QueryEntitySetOrFailureAsync<DbContact>(expectedQuery, cancellationToken), Times.Once);
    }

    [Fact]
    public static async Task GetLastAsync_DbResultIsFailure_ExpectUnknownFailure()
    {
        var sourceException = new Exception("Some exception message");
        var dbFailure = sourceException.ToFailure("Some text");

        var mockSqlApi = BuildMockSqlApi(dbFailure);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetLastAsync(SomeLastContactSetGetInput, default);
        var expected = Failure.Create(ContactSetGetFailureCode.Unknown, "Some text", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmContactApiTestSource.OutputLastGetTestData), MemberType = typeof(CrmContactApiTestSource))]
    internal static async Task GetLastAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        FlatArray<DbContact> dbIncidentContacts, LastContactSetGetOut expected)
    {
        var mockSqlApi = BuildMockSqlApi(dbIncidentContacts);
        var api = new CrmContactApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetLastAsync(SomeLastContactSetGetInput, default);

        Assert.StrictEqual(expected, actual);
    }
}