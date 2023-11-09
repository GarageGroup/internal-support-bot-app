using System;
using System.Collections.Generic;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCreateFlowOption
{
    public IncidentCreateFlowOption(string incidentCardUrlTemplate, int dbRequestPeriodInDays)
    {
        IncidentCardUrlTemplate = incidentCardUrlTemplate.OrEmpty();
        DbRequestPeriodInDays = dbRequestPeriodInDays;
    }

    public string IncidentCardUrlTemplate { get; }

    public int DbRequestPeriodInDays { get; }

    public FlatArray<KeyValuePair<string, string>> GptTraceData { get; init; }
}