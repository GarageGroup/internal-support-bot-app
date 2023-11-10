using System;

namespace GarageGroup.Internal.Support;

public readonly record struct LastContactSetGetOut
{
    public required FlatArray<ContactItemOut> Contacts { get; init; }
}