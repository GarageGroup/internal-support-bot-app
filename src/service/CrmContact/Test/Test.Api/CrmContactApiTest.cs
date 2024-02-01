using System;
using System.Text.Json;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

public static partial class CrmContactApiTest
{
    private static readonly ContactSetSearchIn SomeContactSetSearchInput
        =
        new(
            searchText: "Some search text",
            customerId: new("3a727567-8e28-4e82-86fe-dbad1c74ef6f"))
        {
            Top = 5
        };

    private static readonly LastContactSetGetIn SomeLastContactSetGetInput
        =
        new(
            customerId: new("70bbc008-558d-434c-a8e2-ea17ee121791"),
            userId: new("41113f2a-8a51-4d45-9623-750195e4618c"),
            top: 3);

    private static readonly FlatArray<DataverseSearchItem> SomeDataverseItems
        =
        [
            new(
                searchScore: 812634.97,
                objectId: new("2e96911b-1d7a-4258-b8a4-24c3c542d33b"),
                entityName: "Some Entity Name",
                extensionData:
                [
                    new("name", new(JsonSerializer.SerializeToElement("Some Name"))),
                    new("fullname", new(JsonSerializer.SerializeToElement("Some FullName")))
                ]),
            new(
                searchScore: -305.1,
                objectId: new("adf95291-bcc3-494d-a169-fcde671da606"),
                entityName: "Some second entity",
                extensionData: default)
        ];

    private static FlatArray<DbContact> SomeDbContacts
        =>
        [
            new()
            {
                Id = new("4cbbd503-f1f7-4b32-bf0a-52248eae50bd"),
                Name = "First Contact"
            },
            new()
            {
                Id = new("121d8c04-7407-4228-b7b7-c3ad0b362fcf"),
                Name = "Second Contact"
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
        in Result<FlatArray<DbContact>, Failure<Unit>> result)
    {
        var mock = new Mock<ISqlQueryEntitySetSupplier>();

        _ = mock
            .Setup(s => s.QueryEntitySetOrFailureAsync<DbContact>(It.IsAny<IDbQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}