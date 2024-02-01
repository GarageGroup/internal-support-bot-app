using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmOwner.Test;

partial class CrmOwnerApiTestSource
{
    public static TheoryData<FlatArray<DbIncidentOwner>, LastOwnerSetGetOut> OutputLastGetTestData
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
                        OwnerId = new("be4c871d-bc86-4a3b-ae20-56cee1dc0bdb"),
                        OwnerName = "Emily Carter"
                    },
                    new()
                    {
                        OwnerId = new("43645ed7-75b7-4e9b-abbe-505b04c646bd"),
                        OwnerName = null
                    },
                    new()
                    {
                        OwnerId = new("5bfdf682-cde0-404d-8461-80cfd795c403"),
                        OwnerName = "Charlotte Wilson"
                    }
                ],
                new()
                {
                    Owners =
                    [
                        new(new("be4c871d-bc86-4a3b-ae20-56cee1dc0bdb"), "Emily Carter"),
                        new(new("43645ed7-75b7-4e9b-abbe-505b04c646bd"), string.Empty),
                        new(new("5bfdf682-cde0-404d-8461-80cfd795c403"), "Charlotte Wilson")
                    ]
                }
            }
        };
}