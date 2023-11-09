using System;

namespace GarageGroup.Internal.Support;

public readonly record struct ContactSetSearchOut
{
    public required FlatArray<ContactItemOut> Contacts { get; init; }
}