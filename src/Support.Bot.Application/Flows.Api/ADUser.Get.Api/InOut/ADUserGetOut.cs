using System;

namespace GGroupp.Internal.Support.Bot;

public sealed record ADUserGetOut
{
    public ADUserGetOut(Guid id) => Id = id;

    public Guid Id { get; }
}