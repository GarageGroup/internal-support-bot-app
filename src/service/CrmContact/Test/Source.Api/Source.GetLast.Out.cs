using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTestSource
{
    public static TheoryData<FlatArray<DbContact>, LastContactSetGetOut> OutputLastGetTestData
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
                        Id = new("7f616d2a-1aa2-4d89-be5f-e1da9dbd7a2e"),
                        Name = "John Smith"
                    },
                    new()
                    {
                        Id = new("7f38b958-1f3a-4664-bd8f-91e38027b04d"),
                        Name = null
                    }
                ],
                new()
                {
                    Contacts =
                    [
                        new(new("7f616d2a-1aa2-4d89-be5f-e1da9dbd7a2e"), "John Smith"),
                        new(new("7f38b958-1f3a-4664-bd8f-91e38027b04d"), string.Empty)
                    ]
                }
            }
        };
}