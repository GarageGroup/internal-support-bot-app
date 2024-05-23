namespace GarageGroup.Internal.Support;

internal readonly record struct IncidentGptFlowState
{
    public string? Title { get; init; }

    public IncidentCaseTypeCode? CaseTypeCode { get; init; }

    public string? SourceMessage { get; init; }
}