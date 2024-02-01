using System;
using System.Text.Json;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Support.Service.CrmOwner.Test;

public static partial class CrmOwnerApiTest
{
    private static readonly LastOwnerSetGetIn SomeLastOwnerSetGetInput
        =
        new(
            customerId: new("6af020cf-6b48-40b1-9817-5411979991f8"),
            userId: new("209dfe14-35d6-43f2-be98-75d9aa2c6685"),
            top: 27);

    private static readonly FlatArray<DataverseSearchItem> SomeDataverseItems
        =
        [
            new(
                searchScore: 251.71,
                objectId: new("5cc6ad62-9130-4f08-b69a-44f8b12247fd"),
                entityName: "Some entity",
                extensionData: default),
            new(
                searchScore: -7.1,
                objectId: new("d3e9cb3b-f4fb-4ae6-88ea-9922df5794b9"),
                entityName: "Some second entity",
                extensionData:
                [
                    new("name", new(JsonSerializer.SerializeToElement("Some Name"))),
                    new("fullname", new(JsonSerializer.SerializeToElement("Some FullName")))
                ])
        ];

    private static FlatArray<DbIncidentOwner> SomeDbIncidentOwners
        =>
        [
            new()
            {
                OwnerId = new("67e28e8a-466a-4276-bb50-9f5017906e3e"),
                OwnerName = "First"
            },
            new()
            {
                OwnerId = new("04131d2a-8a8d-4db1-8913-faa9a1186645"),
                OwnerName = "Second"
            }
        ];

    private static Mock<IDataverseSearchSupplier> CreateMockDataverseApi(
        in Result<DataverseSearchOut, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IDataverseSearchSupplier>();

        _ = mock
            .Setup(s => s.SearchAsync(It.IsAny<DataverseSearchIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    private static Mock<ISqlQueryEntitySetSupplier> BuildMockSqlApi(
        in Result<FlatArray<DbIncidentOwner>, Failure<Unit>> result)
    {
        var mock = new Mock<ISqlQueryEntitySetSupplier>();

        _ = mock
            .Setup(s => s.QueryEntitySetOrFailureAsync<DbIncidentOwner>(It.IsAny<IDbQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}