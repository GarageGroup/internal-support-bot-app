using System;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentCreateFlowState
{
    public required Guid BotUserId { get; init; }

    public required string? BotUserName { get; init; }

    public required string? Description { get; init; }

    public FlatArray<string> PhotoIdSet { get; init; }

    public FlatArray<PictureState> Pictures { get; init; }

    public SourceSenderState? SourceSender { get; init; }

    public IncidentCustomerState? Customer { get; init; }

    public IncidentContactState? Contact { get; init; }

    public string? Title { get; init; }

    public CaseTypeState? CaseType { get; init; }

    public PriorityState? Priority { get; init; }

    public IncidentOwnerState? Owner { get; init; }

    public int TemporaryMessageId { get; init; }

    public Guid IncidentId { get; init; }

    public IncidentGptFlowState Gpt { get; init; }

    public bool WithoutConfirmation { get; init; }

    public bool IsRepeated { get; init; }
}