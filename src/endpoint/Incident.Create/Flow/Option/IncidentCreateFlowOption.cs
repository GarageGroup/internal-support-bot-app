using System;
using System.Collections.Generic;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCreateFlowOption
{
    public IncidentCreateFlowOption(string incidentCardUrlTemplate, int dbRequestPeriodInDays, string webAppUrl)
    {
        IncidentCardUrlTemplate = incidentCardUrlTemplate.OrEmpty();
        DbRequestPeriodInDays = dbRequestPeriodInDays;
        WebAppUrl = webAppUrl.OrEmpty();
    }

    public string IncidentCardUrlTemplate { get; }

    public int DbRequestPeriodInDays { get; }

    public string WebAppUrl { get; }

    public FlatArray<KeyValuePair<string, string>> GptTraceData { get; init; }
}