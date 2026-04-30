using System;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Support.Service.CrmProject.Test;

public static partial class CrmProjectApiTest
{
    private static readonly ProjectSetGetIn SomeProjectSetGetInput
        =
        new(
            customerId: new("6af020cf-6b48-40b1-9817-5411979991f8"));

    private static FlatArray<DbProject> SomeDbProjects
        =>
        [
            new()
            {
                ProjectId = new("67e28e8a-466a-4276-bb50-9f5017906e3e"),
                ProjectName = "First"
            },
            new()
            {
                ProjectId = new("04131d2a-8a8d-4db1-8913-faa9a1186645"),
                ProjectName = "Second"
            }
        ];

    private static Mock<ISqlQueryEntitySetSupplier> BuildMockSqlApi(
        in Result<FlatArray<DbProject>, Failure<Unit>> result)
    {
        var mock = new Mock<ISqlQueryEntitySetSupplier>();

        _ = mock
            .Setup(s => s.QueryEntitySetOrFailureAsync<DbProject>(It.IsAny<IDbQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}