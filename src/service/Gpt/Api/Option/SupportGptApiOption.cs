using System;

namespace GarageGroup.Internal.Support;

public sealed record class SupportGptApiOption
{
    public SupportGptApiOption(FlatArray<ChatMessageOption> chatMessages)
        =>
        ChatMessages = chatMessages;

    public FlatArray<ChatMessageOption> ChatMessages { get; }

    public int? MaxTokens { get; init; }

    public decimal? Temperature { get; init; }

    public ChatMessageOption? CaseTypeTemplate { get; init; }

    public bool IsImageProcessing { get; init; }
}