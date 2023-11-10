using System;

namespace GarageGroup.Internal.Support;

public readonly record struct LastOwnerSetGetOut
{
    public required FlatArray<OwnerItemOut> Owners { get; init; }
}