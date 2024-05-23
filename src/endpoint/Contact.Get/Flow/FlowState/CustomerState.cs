using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

internal sealed record class CustomerState
{
    public CustomerState(Guid id, [AllowNull] string name)
    {
        Id = id;
        Name = name.OrEmpty();
    }

    public Guid Id { get; }

    public string Name { get; }
}