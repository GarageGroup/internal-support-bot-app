using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

internal sealed record class PriorityValue
{
    public PriorityValue(IncidentPriorityCode code, [AllowNull] string name)
    {
        Code = code;
        Name = name.OrEmpty();
    }

    public IncidentPriorityCode Code { get; }

    public string Name { get; }
}