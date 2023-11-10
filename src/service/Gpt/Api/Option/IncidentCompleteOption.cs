using System;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCompleteOption
{
    public IncidentCompleteOption(FlatArray<ChatMessageOption> chatMessages)
        =>
        ChatMessages = chatMessages;

    public FlatArray<ChatMessageOption> ChatMessages { get; }

    public int? MaxTokens { get; init; }

    public decimal? Temperature { get; init; }
}