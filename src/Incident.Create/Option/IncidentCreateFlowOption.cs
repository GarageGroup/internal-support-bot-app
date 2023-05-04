using System;
using System.Collections.Generic;

namespace GGroupp.Internal.Support;

public sealed record class IncidentCreateFlowOption
{
    public IncidentCreateFlowOption(string incidentCardUrlTemplate)
        =>
        IncidentCardUrlTemplate = incidentCardUrlTemplate.OrEmpty();

    public string IncidentCardUrlTemplate { get; }

    public FlatArray<KeyValuePair<string, string>> GptTraceData { get; init; }
}