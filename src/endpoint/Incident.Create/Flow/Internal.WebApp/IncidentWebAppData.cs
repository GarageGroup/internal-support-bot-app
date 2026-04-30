using System;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentWebAppData
{
    public string? Title { get; init; }

    public CustomerWebAppData? Customer { get; init; }

    public ContactWebAppData? Contact { get; init; }

    public ProjectWebAppData? Project { get; init; }

    public FlatArray<ProjectWebAppData> Projects { get; init; }

    public IncidentCaseTypeCode CaseTypeCode { get; init; }

    public IncidentPriorityCode PriorityCode { get; init; }

    public OwnerWebAppData? Owner { get; init; }

    public FlatArray<OwnerWebAppData> Owners { get; init; }

    public string? Description { get; init; }

    public FlatArray<string> FileNames { get; init; }
}
