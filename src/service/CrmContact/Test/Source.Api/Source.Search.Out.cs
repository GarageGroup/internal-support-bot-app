using System.Text.Json;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTestSource
{
    public static TheoryData<DataverseSearchOut, ContactSetSearchOut> OutputSearchTestData
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
                            searchScore: -81263.91,
                            objectId: new("604fae90-7894-48ea-92bf-e888bf0ce6ca"),
                            entityName: "First entity name",
                            extensionData: default),
                        new(
                            searchScore: 1000,
                            objectId: new("eaf4a5e1-3303-4ec1-84cd-626b3828b13b"),
                            entityName: "SecondEntityName",
                            extensionData:
                            [
                                new("fullName", new(JsonSerializer.SerializeToElement("Some value"))),
                                new("fullname", new(JsonSerializer.SerializeToElement("Some Full Name")))
                            ])
                    ]),
                new()
                {
                    Contacts =
                    [
                        new(new("604fae90-7894-48ea-92bf-e888bf0ce6ca"), string.Empty),
                        new(new("eaf4a5e1-3303-4ec1-84cd-626b3828b13b"), "Some Full Name")
                    ]
                }
            }
        };
}