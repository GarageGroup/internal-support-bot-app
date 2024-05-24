using System;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.ApplicationInsights;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

public static class IncidentCreateDependency
{
    public static Dependency<IChatCommand<IncidentCreateCommandIn, Unit>> UseIncidentCreateCommand(
        this Dependency<ICrmIncidentApi, ICrmOwnerApi, ISupportGptApi, TelemetryClient?, IncidentCreateFlowOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IChatCommand<IncidentCreateCommandIn, Unit>>(CreateCommand);

        static IncidentCreateCommand CreateCommand(
            ICrmIncidentApi incidentApi,
            ICrmOwnerApi ownerApi,
            ISupportGptApi gptApi,
            TelemetryClient? telemetryClient,
            IncidentCreateFlowOption option)
        {
            ArgumentNullException.ThrowIfNull(incidentApi);
            ArgumentNullException.ThrowIfNull(ownerApi);
            ArgumentNullException.ThrowIfNull(gptApi);
            ArgumentNullException.ThrowIfNull(option);

            return new(incidentApi, ownerApi, gptApi, telemetryClient, option);
        }
    }
}