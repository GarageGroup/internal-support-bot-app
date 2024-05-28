using System;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCreateCommandIn : IChatCommandIn<Unit>
{
    public static string Type { get; } = "IncidentCreate";

    public IncidentCreateCommandIn(string? description)
        =>
        Description = description;

    public string? Description { get; }

    public FlatArray<string> PhotoIdSet { get; init; }

    public FlatArray<string> DocumentIdSet { get; init; }

    public BotUser? SourceSender { get; init; }
}