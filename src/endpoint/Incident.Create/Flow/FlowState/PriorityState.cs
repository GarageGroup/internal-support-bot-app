using System;

namespace GarageGroup.Internal.Support;

internal sealed record class PriorityState
{
    public PriorityState(IncidentPriorityCode code, string title)
    {
        Code = code;
        Title = title.OrEmpty();
    }

    public IncidentPriorityCode Code { get; }

    public string Title { get; }
}