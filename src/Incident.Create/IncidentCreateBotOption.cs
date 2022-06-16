using System;

namespace GGroupp.Internal.Support;

public sealed record class IncidentCreateBotOption
{
    public IncidentCreateBotOption(string incidentCardUrlTemplate)
        =>
        IncidentCardUrlTemplate = incidentCardUrlTemplate.OrEmpty();

    public string IncidentCardUrlTemplate { get; }
}