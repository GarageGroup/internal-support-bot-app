using System;

namespace GarageGroup.Internal.Support;

public sealed record class LastOwnerSetGetIn
{
    public LastOwnerSetGetIn(Guid customerId, Guid userId, int top)
    {
        CustomerId = customerId;
        UserId = userId;
        Top = top;
    }

    public Guid CustomerId { get; }

    public Guid UserId { get; }

    public int Top { get; }
}