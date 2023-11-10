using System;

namespace GarageGroup.Internal.Support;

public readonly record struct OwnerSetSearchOut
{
    public required FlatArray<OwnerItemOut> Owners { get; init; }
}