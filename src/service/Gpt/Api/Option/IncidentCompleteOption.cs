using System;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCompleteOption
{
    public IncidentCompleteOption(string model)
        =>
        Model = model.OrEmpty();

    public string Model { get; }

    public required FlatArray<ChatMessageOption> ChatMessages { get; init; }

    public int? MaxTokens { get; init; }

    public decimal? Temperature { get; init; }
}