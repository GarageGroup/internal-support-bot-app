using System;

namespace GarageGroup.Internal.Support;

public readonly record struct ProjectSetGetOut
{
    public required FlatArray<ProjectItem> Projects { get; init; }
}