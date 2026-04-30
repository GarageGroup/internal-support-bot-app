using System;

namespace GarageGroup.Internal.Support;

public sealed record class ProjectSetGetIn
{
    public ProjectSetGetIn(Guid customerId)
        =>
        CustomerId = customerId;
    
    public Guid CustomerId { get; }
}
