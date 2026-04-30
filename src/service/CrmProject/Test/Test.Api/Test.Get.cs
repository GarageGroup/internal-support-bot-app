using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmProject.Test;

partial class CrmProjectApiTest
{
    [Fact]
    public static async Task GetAsync_InputIsNotNull_ExpectSqlApiCalledOnce()
    {
        var mockSqlApi = BuildMockSqlApi(SomeDbProjects);
        var api = new CrmProjectApi(mockSqlApi.Object);

        var input = new ProjectSetGetIn(
            customerId: new("4a8f89de-3a0f-4952-9a92-7421b2a72405"));

        _ = await api.GetAsync(input, TestContext.Current.CancellationToken);

        var expectedQuery = new DbSelectQuery("gg_project", "p")
        {
            SelectedFields = new("p.gg_projectid AS ProjectId", "p.gg_name AS ProjectName"),
            Filter = new DbCombinedFilter(DbLogicalOperator.And)
            {
                Filters =
                [
                    new DbRawFilter("p.statecode = 0"),
                    new DbParameterFilter(
                        "p.gg_clientid", DbFilterOperator.Equal, Guid.Parse("4a8f89de-3a0f-4952-9a92-7421b2a72405"), "clientId")
                ]
            }
        };

        mockSqlApi.Verify(a => a.QueryEntitySetOrFailureAsync<DbProject>(expectedQuery, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task GetAsync_DbResultIsFailure_ExpectUnknownFailure()
    {
        var sourceException = new Exception("Some error message");
        var dbFailure = sourceException.ToFailure("Some failure message");

        var mockSqlApi = BuildMockSqlApi(dbFailure);
        var api = new CrmProjectApi(mockSqlApi.Object);

        var actual = await api.GetAsync(SomeProjectSetGetInput, TestContext.Current.CancellationToken);

        Assert.StrictEqual(dbFailure, actual);
    }

    [Theory]
    [MemberData(nameof(CrmProjectApiTestSource.OutputGetTestData), MemberType = typeof(CrmProjectApiTestSource))]
    internal static async Task GetAsync_DbResultIsSuccess_ExpectSuccess(
        FlatArray<DbProject> dbProjects, ProjectSetGetOut expected)
    {
        var mockSqlApi = BuildMockSqlApi(dbProjects);
        var api = new CrmProjectApi(mockSqlApi.Object);

        var actual = await api.GetAsync(SomeProjectSetGetInput, TestContext.Current.CancellationToken);

        Assert.StrictEqual(expected, actual);
    }
}
