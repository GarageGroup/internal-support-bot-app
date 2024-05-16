namespace GarageGroup.Internal.Support;

public readonly record struct IncidentCompleteOut
{
    public string? Title { get; init; }

    public IncidentCaseTypeCode CaseTypeCode { get; init; }
}