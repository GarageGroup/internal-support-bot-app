using System;

namespace GGroupp.Internal.Support;

public sealed record class IncidentCreateBotOption
{
    public IncidentCreateBotOption(string incidentCardUrlTemplate, int? caseOriginCode)
    {
        IncidentCardUrlTemplate = incidentCardUrlTemplate.OrEmpty();
        CaseOriginCode = caseOriginCode;
    }

    public string IncidentCardUrlTemplate { get; }

    public int? CaseOriginCode { get; }
}