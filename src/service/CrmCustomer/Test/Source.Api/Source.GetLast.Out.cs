using System;
using System.Collections.Generic;

namespace GarageGroup.Internal.Support.Service.CrmCustomer.Test;

partial class CrmCustomerApiTestSource
{
    public static IEnumerable<object[]> OutputLastGetTestData
        =>
        new object[][]
        {
            [
                default(FlatArray<DbIncidentCustomer>),
                default(LastCustomerSetGetOut)
            ],
            [
                new DbIncidentCustomer[]
                {
                    new()
                    {
                        CustomerId = Guid.Parse("662f35a5-4df2-44b7-ae2c-3f4c63d0bcb9"),
                        CustomerName = null
                    },
                    new()
                    {
                        CustomerId = Guid.Parse("a66feff6-e748-4f08-97dd-80cd64c7ea34"),
                        CustomerName = "Acme Corporation"
                    },
                    new()
                    {
                        CustomerId = Guid.Parse("4cbbd503-f1f7-4b32-bf0a-52248eae50bd"),
                        CustomerName = "Jane Doe"
                    }
                },
                new LastCustomerSetGetOut
                {
                    Customers = new CustomerItemOut[]
                    {
                        new(Guid.Parse("662f35a5-4df2-44b7-ae2c-3f4c63d0bcb9"), string.Empty),
                        new(Guid.Parse("a66feff6-e748-4f08-97dd-80cd64c7ea34"), "Acme Corporation"),
                        new(Guid.Parse("4cbbd503-f1f7-4b32-bf0a-52248eae50bd"), "Jane Doe")
                    }
                }
            ]
        };
}