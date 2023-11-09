using System;

namespace GarageGroup.Internal.Support;

public sealed record class LastCustomerSetGetIn
{
    public LastCustomerSetGetIn(Guid userId, DateTime minCreationTime, int top)
    {
        UserId = userId;
        MinCreationTime = minCreationTime;
        Top = top;
    }

    public Guid UserId { get; }

    public DateTime MinCreationTime { get; }

    public int Top { get; }
}