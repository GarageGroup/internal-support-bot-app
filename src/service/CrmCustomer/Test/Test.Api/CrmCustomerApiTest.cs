using System;
using System.Text.Json;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

public static partial class CrmCustomerApiTest
{
    private static readonly LastCustomerSetGetIn SomeLastCustomerSetGetInput
        =
        new(
            userId: new("cbe75335-98fa-41f1-a6b9-7aca6e31d03a"),
            minCreationTime: new(2021, 11, 07, 03, 27, 45),
            top: 3);

    private static readonly FlatArray<DataverseSearchItem> SomeDataverseItems
        =
        [
            new(
                searchScore: 175.93,
                objectId: new("613e25fb-60bc-4b2b-be95-bddad08d4b14"),
                entityName: "Some entity",
                extensionData:
                [
                    new("name", new(JsonSerializer.SerializeToElement("Some Name"))),
                    new("fullname", new(JsonSerializer.SerializeToElement("Some FullName")))
                ]),
            new(
                searchScore: 20,
                objectId: new("c391bf21-379f-4e78-9198-27d677dd300f"),
                entityName: "Some second entity",
                extensionData: default)
        ];

    private static FlatArray<DbIncidentCustomer> SomeDbIncidentCustomers
        =>
        [
            new()
            {
                CustomerId = new("070bf50d-ba93-42a6-8064-bea94ba1e017"),
                CustomerName = "Customer One"
            },
            new()
            {
                CustomerId = new("39a7eef1-d2c3-4240-991a-3f237890a4de"),
                CustomerName = "Customer Two"
            }
        ];

    private static Mock<IDataverseSearchSupplier> BuildMockDataverseApi(
        in Result<DataverseSearchOut, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IDataverseSearchSupplier>();

        _ = mock
            .Setup(s => s.SearchAsync(It.IsAny<DataverseSearchIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    private static Mock<ISqlQueryEntitySetSupplier> BuildMockSqlApi(
        in Result<FlatArray<DbIncidentCustomer>, Failure<Unit>> result)
    {
        var mock = new Mock<ISqlQueryEntitySetSupplier>();

        _ = mock
            .Setup(s => s.QueryEntitySetOrFailureAsync<DbIncidentCustomer>(It.IsAny<IDbQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}