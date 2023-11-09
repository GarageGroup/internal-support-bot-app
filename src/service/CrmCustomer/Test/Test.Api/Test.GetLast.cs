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
    public static async Task GetLastAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockSqlApi = BuildMockSqlApi(SomeDbIncidentCustomers);
        var api = new CrmCustomerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

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
        var mockSqlApi = BuildMockSqlApi(SomeDbIncidentCustomers);
        var api = new CrmCustomerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var input = new LastCustomerSetGetIn(
            userId: Guid.Parse("f51ceb91-f74d-4ea6-b179-c4af139d2f6f"),
            minCreationTime: new(2023, 09, 17, 23, 16, 51),
            top: 7);

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.GetLastAsync(input, cancellationToken);

        var expectedQuery = new DbSelectQuery("incident", "i")
        {
            Top = 7,
            SelectedFields = new(
                "i.customerid AS CustomerId",
                "a.name AS CustomerName",
                "MAX(i.createdon) AS MaxCreatedOn",
                "MAX(CASE WHEN i.createdby = 'f51ceb91-f74d-4ea6-b179-c4af139d2f6f' THEN i.createdon ELSE NULL END) AS MaxCurrentUserCreatedOn"),
            JoinedTables = new DbJoinedTable[]
            {
                new(DbJoinType.Inner, "account", "a", new DbRawFilter("a.accountid = i.customerid"))
            },
            Filter = new DbCombinedFilter(DbLogicalOperator.And)
            {
                Filters = new IDbFilter[]
                {
                    new DbRawFilter("i.customerid IS NOT NULL"),
                    new DbParameterFilter("i.createdon", DbFilterOperator.GreaterOrEqual, new DateTime(2023, 09, 17, 23, 16, 51), "minDate")
                }
            },
            GroupByFields = new("i.customerid", "a.name"),
            Orders = new DbOrder[]
            {
                new("MaxCurrentUserCreatedOn", DbOrderType.Descending),
                new("MaxCreatedOn", DbOrderType.Descending)
            }
        };

        mockSqlApi.Verify(a => a.QueryEntitySetOrFailureAsync<DbIncidentCustomer>(expectedQuery, cancellationToken), Times.Once);
    }

    [Fact]
    public static async Task GetLastAsync_DbResultIsFailure_ExpectUnknownFailure()
    {
        var sourceException = new Exception("Some error text");
        var dbFailure = sourceException.ToFailure("Some Failure message");

        var mockSqlApi = BuildMockSqlApi(dbFailure);
        var api = new CrmCustomerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetLastAsync(SomeLastCustomerSetGetInput, default);
        var expected = Failure.Create(CustomerSetGetFailureCode.Unknown, "Some Failure message", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmCustomerApiTestSource.OutputLastGetTestData), MemberType = typeof(CrmCustomerApiTestSource))]
    internal static async Task GetLastAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        FlatArray<DbIncidentCustomer> dbIncidentCustomers, LastCustomerSetGetOut expected)
    {
        var mockSqlApi = BuildMockSqlApi(dbIncidentCustomers);
        var api = new CrmCustomerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetLastAsync(SomeLastCustomerSetGetInput, default);

        Assert.StrictEqual(expected, actual);
    }
}