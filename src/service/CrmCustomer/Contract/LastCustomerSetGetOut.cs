using System;

namespace GarageGroup.Internal.Support;

public readonly record struct LastCustomerSetGetOut
{
    public required FlatArray<CustomerItemOut> Customers { get; init; }
}