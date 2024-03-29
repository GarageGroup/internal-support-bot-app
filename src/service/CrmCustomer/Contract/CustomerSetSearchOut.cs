using System;

namespace GarageGroup.Internal.Support;

public readonly record struct CustomerSetSearchOut
{
    public required FlatArray<CustomerItemOut> Customers { get; init; }
}