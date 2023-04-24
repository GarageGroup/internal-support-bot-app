using System;

namespace GGroupp.Internal.Support;

public sealed record class IncidentCreateFlowOption
{
    public IncidentCreateFlowOption(string incidentCardUrlTemplate)
        =>
        IncidentCardUrlTemplate = incidentCardUrlTemplate.OrEmpty();

    public string IncidentCardUrlTemplate { get; }
}