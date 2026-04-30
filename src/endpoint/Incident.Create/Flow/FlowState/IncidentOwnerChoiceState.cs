using System;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentOwnerChoiceState
{
    public IncidentOwnerChoiceState(IncidentOwnerState owner, FlatArray<IncidentOwnerState> foundOwners)
    {
        Owner = owner;
        FoundOwners = foundOwners;
    }

    public IncidentOwnerState Owner { get; }

    public FlatArray<IncidentOwnerState> FoundOwners { get; }
}
