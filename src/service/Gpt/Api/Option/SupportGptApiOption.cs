using System;

namespace GarageGroup.Internal.Support;

public sealed record class SupportGptApiOption
{
    public SupportGptApiOption(string apiKey, string model, IncidentCompleteOption incidentComplete)
    {
        ApiKey = apiKey.OrEmpty();
        Model = model.OrEmpty();
        IncidentComplete = incidentComplete;
    }

    public SupportGptApiOption(string apiKey, SupportAzureGptApiOption azureGpt, IncidentCompleteOption incidentComplete)
    {
        ApiKey = apiKey.OrEmpty();
        AzureGpt = azureGpt;
        IncidentComplete = incidentComplete;
    }

    public string ApiKey { get; }

    public string? Model { get; }

    public SupportAzureGptApiOption? AzureGpt { get; }

    public IncidentCompleteOption IncidentComplete { get; }
}