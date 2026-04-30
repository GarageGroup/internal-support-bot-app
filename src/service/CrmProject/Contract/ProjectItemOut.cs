using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class ProjectItemOut
{
    public ProjectItemOut(Guid id, [AllowNull] string name)
    {
        Id = id;
        Name = name.OrEmpty();
    }

    public Guid Id { get; }

    public string Name { get; }
}
