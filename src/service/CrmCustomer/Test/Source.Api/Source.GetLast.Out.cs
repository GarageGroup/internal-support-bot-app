using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

partial class CrmCustomerApiTestSource
{
    public static TheoryData<FlatArray<DbIncidentCustomer>, LastCustomerSetGetOut> OutputLastGetTestData
        =>
        new()
        {
            {
                default,
                default
            },
            {
                [
                    new()
                    {
                        CustomerId = new("662f35a5-4df2-44b7-ae2c-3f4c63d0bcb9"),
                        CustomerName = null
                    },
                    new()
                    {
                        CustomerId = new("a66feff6-e748-4f08-97dd-80cd64c7ea34"),
                        CustomerName = "Acme Corporation"
                    },
                    new()
                    {
                        CustomerId = new("4cbbd503-f1f7-4b32-bf0a-52248eae50bd"),
                        CustomerName = "Jane Doe"
                    }
                ],
                new()
                {
                    Customers =
                    [
                        new(new("662f35a5-4df2-44b7-ae2c-3f4c63d0bcb9"), string.Empty),
                        new(new("a66feff6-e748-4f08-97dd-80cd64c7ea34"), "Acme Corporation"),
                        new(new("4cbbd503-f1f7-4b32-bf0a-52248eae50bd"), "Jane Doe")
                    ]
                }
            }
        };
}