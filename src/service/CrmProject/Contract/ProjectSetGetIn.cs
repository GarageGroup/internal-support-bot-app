using System;

namespace GarageGroup.Internal.Support;

public readonly record struct ProjectSetGetIn
{
    public ProjectSetGetIn(Guid customerId)
        =>
        CustomerId = customerId;
    
    public Guid CustomerId { get; }
}