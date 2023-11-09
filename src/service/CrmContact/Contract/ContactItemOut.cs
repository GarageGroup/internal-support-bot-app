using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class ContactItemOut
{
    public ContactItemOut(Guid id, [AllowNull] string fullName)
    {
        Id = id;
        FullName = fullName.OrEmpty();
    }

    public Guid Id { get; }

    public string FullName { get; }
}