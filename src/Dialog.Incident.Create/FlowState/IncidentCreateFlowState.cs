using System;

namespace GGroupp.Internal.Support.Bot;

internal sealed record IncidentCreateFlowState
{
    public Guid OwnerId { get; init; }

    public Guid CustomerId { get; init; }

    public string? CustomerTitle { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public int CaseType { get; init; }
}
