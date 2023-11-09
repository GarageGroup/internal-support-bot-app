using System;
using System.Text.Json;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Support.Service.CrmUser.Test;

public static partial class CrmUserApiTest
{
    private static readonly FlatArray<DataverseSearchItem> SomeDataverseItems
        =
        new DataverseSearchItem[]
        {
            new(
                searchScore: 251.71,
                objectId: Guid.Parse("5cc6ad62-9130-4f08-b69a-44f8b12247fd"),
                entityName: "Some entity",
                extensionData: default),
            new(
                searchScore: -7.1,
                objectId: Guid.Parse("d3e9cb3b-f4fb-4ae6-88ea-9922df5794b9"),
                entityName: "Some second entity",
                extensionData: new(
                    new("name", new(JsonSerializer.SerializeToElement("Some Name"))),
                    new("fullname", new(JsonSerializer.SerializeToElement("Some FullName")))))
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