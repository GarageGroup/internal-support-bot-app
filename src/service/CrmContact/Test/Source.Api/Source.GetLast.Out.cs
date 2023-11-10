﻿using System;
using System.Collections.Generic;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTestSource
{
    public static IEnumerable<object[]> OutputLastGetTestData
        =>
        new object[][]
        {
            [
                default(FlatArray<DbContact>),
                default(LastContactSetGetOut)
            ],
            [
                new DbContact[]
                {
                    new()
                    {
                        Id = Guid.Parse("7f616d2a-1aa2-4d89-be5f-e1da9dbd7a2e"),
                        Name = "John Smith"
                    },
                    new()
                    {
                        Id = Guid.Parse("7f38b958-1f3a-4664-bd8f-91e38027b04d"),
                        Name = null
                    }
                },
                new LastContactSetGetOut
                {
                    Contacts = new ContactItemOut[]
                    {
                        new(Guid.Parse("7f616d2a-1aa2-4d89-be5f-e1da9dbd7a2e"), "John Smith"),
                        new(Guid.Parse("7f38b958-1f3a-4664-bd8f-91e38027b04d"), string.Empty)
                    }
                }
            ]
        };
}