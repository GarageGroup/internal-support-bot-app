﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class CommandCustomer
{
    public CommandCustomer(Guid id, [AllowNull] string title)
    {
        Id = id;
        Title = title.OrEmpty();
    }

    public Guid Id { get; }

    public string Title { get; }
}