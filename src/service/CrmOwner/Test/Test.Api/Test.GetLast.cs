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
    public static async Task GetLastAsync_InputIsNull_ExpectArgumentNullException()
    {
        var mockSqlApi = BuildMockSqlApi(SomeDbIncidentOwners);
        var api = new CrmOwnerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

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
        var mockSqlApi = BuildMockSqlApi(SomeDbIncidentOwners);
        var api = new CrmOwnerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var input = new LastOwnerSetGetIn(
            customerId: new("4a8f89de-3a0f-4952-9a92-7421b2a72405"),
            userId: new("71cbe0d9-a715-4bf3-bb0c-bb02e343d569"),
            top: 11);

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.GetLastAsync(input, cancellationToken);

        var expectedQuery = new DbSelectQuery("incident", "i")
        {
            Top = 11,
            SelectedFields = new("i.ownerid AS OwnerId", "u.fullname AS OwnerName", "MAX(i.createdon) AS MaxCreatedOn"),
            Filter = new DbCombinedFilter(DbLogicalOperator.And)
            {
                Filters =
                [
                    new DbParameterFilter(
                        "i.ownerid", DbFilterOperator.Inequal, Guid.Parse("71cbe0d9-a715-4bf3-bb0c-bb02e343d569"), "currentUserId"),
                    new DbParameterFilter(
                        "i.customerid", DbFilterOperator.Equal, Guid.Parse("4a8f89de-3a0f-4952-9a92-7421b2a72405"), "customerId"),
                    new DbRawFilter(
                        "u.isdisabled = 0")
                ]
            },
            JoinedTables =
            [
                new(DbJoinType.Inner, "systemuser", "u", new DbRawFilter("u.systemuserid = i.ownerid"))
            ],
            GroupByFields = new("i.ownerid", "u.fullname"),
            Orders =
            [
                new("MaxCreatedOn", DbOrderType.Descending)
            ]
        };

        mockSqlApi.Verify(a => a.QueryEntitySetOrFailureAsync<DbIncidentOwner>(expectedQuery, cancellationToken), Times.Once);
    }

    [Fact]
    public static async Task GetLastAsync_DbResultIsFailure_ExpectUnknownFailure()
    {
        var sourceException = new Exception("Some error message");
        var dbFailure = sourceException.ToFailure("Some failure message");

        var mockSqlApi = BuildMockSqlApi(dbFailure);
        var api = new CrmOwnerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetLastAsync(SomeLastOwnerSetGetInput, default);
        var expected = Failure.Create(OwnerSetGetFailureCode.Unknown, "Some failure message", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmOwnerApiTestSource.OutputLastGetTestData), MemberType = typeof(CrmOwnerApiTestSource))]
    internal static async Task GetLastAsync_DataverseSearchResultIsSuccess_ExpectSuccess(
        FlatArray<DbIncidentOwner> dbIncidentOwners, LastOwnerSetGetOut expected)
    {
        var mockSqlApi = BuildMockSqlApi(dbIncidentOwners);
        var api = new CrmOwnerApi(Mock.Of<IDataverseSearchSupplier>(), mockSqlApi.Object);

        var actual = await api.GetLastAsync(SomeLastOwnerSetGetInput, default);

        Assert.StrictEqual(expected, actual);
    }
}