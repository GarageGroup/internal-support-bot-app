using Microsoft.ApplicationInsights;

namespace GarageGroup.Internal.Support;

internal sealed partial class IncidentCreateCommand : IIncidentCreateCommand
{
    private readonly ICrmIncidentApi incidentApi;

    private readonly ICrmOwnerApi ownerApi;

    private readonly ISupportGptApi gptApi;

    private readonly TelemetryClient? telemetryClient;

    private readonly IncidentCreateFlowOption option;

    internal IncidentCreateCommand(
        ICrmIncidentApi incidentApi,
        ICrmOwnerApi ownerApi,
        ISupportGptApi gptApi,
        TelemetryClient? telemetryClient,
        IncidentCreateFlowOption option)
    {
        this.incidentApi = incidentApi;
        this.ownerApi = ownerApi;
        this.gptApi = gptApi;
        this.telemetryClient = telemetryClient;
        this.option = option;
    }
}