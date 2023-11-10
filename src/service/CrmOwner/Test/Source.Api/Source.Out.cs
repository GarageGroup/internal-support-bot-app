using System;
using System.Collections.Generic;
using System.Text.Json;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support.Service.CrmOwner.Test;

partial class CrmOwnerApiTestSource
{
    public static IEnumerable<object[]> OutputTestData
        =>
        new object[][]
        {
            [
                new DataverseSearchOut(
                    totalRecordCount: 1,
                    value: default),
                default(OwnerSetSearchOut)
            ],
            [
                new DataverseSearchOut(
                    totalRecordCount: -1,
                    value: new DataverseSearchItem[]
                    {
                        new(
                            searchScore: 217.5,
                            objectId: Guid.Parse("d18417f9-3e5d-4836-8d7d-41cd44baafc4"),
                            entityName: "First Entity name",
                            extensionData: new KeyValuePair<string, DataverseSearchJsonValue>[]
                            {
                                new("fullName", new(JsonSerializer.SerializeToElement("Some Name"))),
                                new("name", new(JsonSerializer.SerializeToElement("Some value"))),
                                new("fullname", new(JsonSerializer.SerializeToElement("First User Name"))),
                                new("some", new(JsonSerializer.SerializeToElement(15)))
                            }),
                        new(
                            searchScore: 155.7,
                            objectId: Guid.Parse("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"),
                            entityName: "Second EntityName",
                            extensionData: new KeyValuePair<string, DataverseSearchJsonValue>[]
                            {
                                new("fullname", new(JsonSerializer.SerializeToElement((string?)null)))
                            }),
                        new(
                            searchScore: -200,
                            objectId: Guid.Parse("e589b17c-b33a-4c32-bacf-f2795192066f"),
                            entityName: "Third Entity Name",
                            extensionData: default)
                    }),
                new OwnerSetSearchOut
                {
                    Owners = new OwnerItemOut[]
                    {
                        new(Guid.Parse("d18417f9-3e5d-4836-8d7d-41cd44baafc4"), "First User Name"),
                        new(Guid.Parse("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"), string.Empty),
                        new(Guid.Parse("e589b17c-b33a-4c32-bacf-f2795192066f"), string.Empty)
                    }
                }
            ]
        };
}