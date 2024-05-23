using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentOwnerState
{
    public IncidentOwnerState(Guid id, [AllowNull] string fullName)
    {
        Id = id;
        FullName = fullName.OrEmpty();
    }

    public Guid Id { get; }

    public string FullName { get; }
}