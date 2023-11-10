using System;
using System.Collections.Generic;

namespace GarageGroup.Internal.Support.Service.CrmOwner.Test;

partial class CrmOwnerApiTestSource
{
    public static IEnumerable<object[]> OutputLastGetTestData
        =>
        new object[][]
        {
            [
                default(FlatArray<DbIncidentOwner>),
                default(LastOwnerSetGetOut)
            ],
            [
                new DbIncidentOwner[]
                {
                    new()
                    {
                        OwnerId = Guid.Parse("be4c871d-bc86-4a3b-ae20-56cee1dc0bdb"),
                        OwnerName = "Emily Carter"
                    },
                    new()
                    {
                        OwnerId = Guid.Parse("43645ed7-75b7-4e9b-abbe-505b04c646bd"),
                        OwnerName = null
                    },
                    new()
                    {
                        OwnerId = Guid.Parse("5bfdf682-cde0-404d-8461-80cfd795c403"),
                        OwnerName = "Charlotte Wilson"
                    }
                },
                new LastOwnerSetGetOut
                {
                    Owners = new OwnerItemOut[]
                    {
                        new(Guid.Parse("be4c871d-bc86-4a3b-ae20-56cee1dc0bdb"), "Emily Carter"),
                        new(Guid.Parse("43645ed7-75b7-4e9b-abbe-505b04c646bd"), string.Empty),
                        new(Guid.Parse("5bfdf682-cde0-404d-8461-80cfd795c403"), "Charlotte Wilson")
                    }
                }
            ]
        };
}