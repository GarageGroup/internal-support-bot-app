using System;

namespace GarageGroup.Internal.Support;

public readonly record struct ProjectSetGetOut
{
    public required FlatArray<ProjectItemOut> Projects { get; init; }
}
