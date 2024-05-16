using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTestSource
{
    public static TheoryData<FlatArray<DbIncident>, ContactGetOut> OutputGetTestData
        =>
        new()
        {
            {
                [
                    new()
                    {
                        ContactId = new("54d20ed9-9435-4d9e-a49e-18d6dd111ab9"),
                        ContactName = "Contact name",
                        CustomerId = new("d748a364-079a-4806-a381-2ab52847e570"),
                        CustomerName = "Customer name"
                    }
                ],
                new(
                    contactId: new("54d20ed9-9435-4d9e-a49e-18d6dd111ab9"),
                    contactName: "Contact name",
                    customerId: new("d748a364-079a-4806-a381-2ab52847e570"),
                    customerName: "Customer name")
            }
        };
}