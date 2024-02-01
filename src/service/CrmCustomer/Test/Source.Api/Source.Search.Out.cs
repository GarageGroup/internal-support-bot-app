using System.Text.Json;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

partial class CrmCustomerApiTestSource
{
    public static TheoryData<DataverseSearchOut, CustomerSetSearchOut> OutputSearchTestData
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
                            searchScore: -917.095,
                            objectId: new("1b91d06f-208d-4c1c-b630-0ee9996a8a59"),
                            entityName: "Some entity name",
                            extensionData: new(
                                new("fullName", new(JsonSerializer.SerializeToElement("Some text"))),
                                new("name", new(JsonSerializer.SerializeToElement("Some title"))))),
                        new(
                            searchScore: 52.1,
                            objectId: new("e481f466-cdae-414a-a94f-7e4cfca32f4b"),
                            entityName: "Another entity name",
                            extensionData: default)
                    ]),
                new()
                {
                    Customers =
                    [
                        new(new("1b91d06f-208d-4c1c-b630-0ee9996a8a59"), "Some title"),
                        new(new("e481f466-cdae-414a-a94f-7e4cfca32f4b"), string.Empty)
                    ]
                }
            }
        };
}