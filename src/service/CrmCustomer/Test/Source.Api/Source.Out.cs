using System;
using System.Collections.Generic;
using System.Text.Json;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

partial class CrmCustomerApiTestSource
{
    public static IEnumerable<object[]> OutputTestData
        =>
        new object[][]
        {
            [
                new DataverseSearchOut(
                    totalRecordCount: 1,
                    value: default),
                default(CustomerSetSearchOut)
            ],
            [
                new DataverseSearchOut(
                    totalRecordCount: -1,
                    value: new DataverseSearchItem[]
                    {
                        new(
                            searchScore: -917.095,
                            objectId: Guid.Parse("1b91d06f-208d-4c1c-b630-0ee9996a8a59"),
                            entityName: "Some entity name",
                            extensionData: new(
                                new("fullName", new(JsonSerializer.SerializeToElement("Some text"))),
                                new("name", new(JsonSerializer.SerializeToElement("Some title"))))),
                        new(
                            searchScore: 52.1,
                            objectId: Guid.Parse("e481f466-cdae-414a-a94f-7e4cfca32f4b"),
                            entityName: "Another entity name",
                            extensionData: default)
                    }),
                new CustomerSetSearchOut
                {
                    Customers = new CustomerItemOut[]
                    {
                        new(Guid.Parse("1b91d06f-208d-4c1c-b630-0ee9996a8a59"), "Some title"),
                        new(Guid.Parse("e481f466-cdae-414a-a94f-7e4cfca32f4b"), string.Empty)
                    }
                }
            ]
        };
}