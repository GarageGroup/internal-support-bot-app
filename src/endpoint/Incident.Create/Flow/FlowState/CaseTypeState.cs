using System;

namespace GarageGroup.Internal.Support;

internal sealed record class CaseTypeState
{
    public CaseTypeState(IncidentCaseTypeCode code, string title)
    {
        Code = code;
        Title = title.OrEmpty();
    }

    public IncidentCaseTypeCode Code { get; }

    public string Title { get; }
}