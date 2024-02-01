using System.Text.Json;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmOwner.Test;

partial class CrmOwnerApiTestSource
{
    public static TheoryData<DataverseSearchOut, OwnerSetSearchOut> OutputSearchTestData
        =>
        new()
        {
            {
                new(
                    totalRecordCount: 1,
                    value: default),
                default
            },
            {
                new(
                    totalRecordCount: -1,
                    value:
                    [
                        new(
                            searchScore: 217.5,
                            objectId: new("d18417f9-3e5d-4836-8d7d-41cd44baafc4"),
                            entityName: "First Entity name",
                            extensionData:
                            [
                                new("fullName", new(JsonSerializer.SerializeToElement("Some Name"))),
                                new("name", new(JsonSerializer.SerializeToElement("Some value"))),
                                new("fullname", new(JsonSerializer.SerializeToElement("First User Name"))),
                                new("some", new(JsonSerializer.SerializeToElement(15)))
                            ]),
                        new(
                            searchScore: 155.7,
                            objectId: new("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"),
                            entityName: "Second EntityName",
                            extensionData:
                            [
                                new("fullname", new(JsonSerializer.SerializeToElement((string?)null)))
                            ]),
                        new(
                            searchScore: -200,
                            objectId: new("e589b17c-b33a-4c32-bacf-f2795192066f"),
                            entityName: "Third Entity Name",
                            extensionData: default)
                    ]),
                new()
                {
                    Owners =
                    [
                        new(new("d18417f9-3e5d-4836-8d7d-41cd44baafc4"), "First User Name"),
                        new(new("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"), string.Empty),
                        new(new("e589b17c-b33a-4c32-bacf-f2795192066f"), string.Empty)
                    ]
                }
            }
        };
}