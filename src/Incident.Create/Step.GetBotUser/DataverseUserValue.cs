using System;

namespace GarageGroup.Internal.Support;

internal sealed record class DataverseUserValue
{
    public required Guid Id { get; init; }

    public string? Name { get; init; }
}