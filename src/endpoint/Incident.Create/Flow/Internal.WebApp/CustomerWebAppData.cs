using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

internal sealed record class CustomerWebAppData
{
    public CustomerWebAppData(Guid id, [AllowNull] string title)
    {
        Id = id;
        Title = title.OrEmpty();
    }

    public Guid Id { get; }

    public string Title { get; }
}