namespace GGroupp.Internal.Support.Bot;

public interface IIncidentCreateFlowConfiguration
{
    string IncidentCardUrlTemplate { get; }

    int? CaseOriginCode { get; }
}

