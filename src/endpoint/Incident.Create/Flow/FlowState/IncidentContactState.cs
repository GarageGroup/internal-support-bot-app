using System;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentContactState
{
    public IncidentContactState(Guid? id, string? fullName)
    {
        Id = id;
        FullName = fullName.OrEmpty();
    }

    public Guid? Id { get; }

    public string? FullName { get; }
}
