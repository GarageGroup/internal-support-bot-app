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
            customerId: Guid.Parse("3a727567-8e28-4e82-86fe-dbad1c74ef6f"))
        {
            Top = 5
        };

    private static readonly FlatArray<DataverseSearchItem> SomeDataverseItems
        =
        new DataverseSearchItem[]
        {
            new(
                searchScore: 812634.97,
                objectId: Guid.Parse("2e96911b-1d7a-4258-b8a4-24c3c542d33b"),
                entityName: "Some Entity Name",
                extensionData: new(
                    new("name", new(JsonSerializer.SerializeToElement("Some Name"))),
                    new("fullname", new(JsonSerializer.SerializeToElement("Some FullName"))))),
            new(
                searchScore: -305.1,
                objectId: Guid.Parse("adf95291-bcc3-494d-a169-fcde671da606"),
                entityName: "Some second entity",
                extensionData: default)
        };

    private static Mock<IDataverseSearchSupplier> CreateMockDataverseApi(
        Result<DataverseSearchOut, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IDataverseSearchSupplier>();

        _ = mock
            .Setup(s => s.SearchAsync(It.IsAny<DataverseSearchIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}