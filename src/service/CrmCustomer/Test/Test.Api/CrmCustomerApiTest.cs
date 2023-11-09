using System;
using System.Text.Json;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

public static partial class CrmCustomerApiTest
{
    private static readonly FlatArray<DataverseSearchItem> SomeDataverseItems
        =
        new DataverseSearchItem[]
        {
            new(
                searchScore: 175.93,
                objectId: Guid.Parse("613e25fb-60bc-4b2b-be95-bddad08d4b14"),
                entityName: "Some entity",
                extensionData: new(
                    new("name", new(JsonSerializer.SerializeToElement("Some Name"))),
                    new("fullname", new(JsonSerializer.SerializeToElement("Some FullName"))))),
            new(
                searchScore: 20,
                objectId: Guid.Parse("c391bf21-379f-4e78-9198-27d677dd300f"),
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