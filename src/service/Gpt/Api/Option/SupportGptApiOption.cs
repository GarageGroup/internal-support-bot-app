using System;

namespace GarageGroup.Internal.Support;

public sealed record class SupportGptApiOption
{
    public SupportGptApiOption(string apiKey, IncidentCompleteOption incidentComplete)
    {
        ApiKey = apiKey.OrEmpty();
        IncidentComplete = incidentComplete;
    }

    public string ApiKey { get; }

    public IncidentCompleteOption IncidentComplete { get; }
}