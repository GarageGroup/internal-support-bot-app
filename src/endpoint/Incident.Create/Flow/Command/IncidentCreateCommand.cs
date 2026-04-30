using System;
using System.Collections.Generic;
using System.Linq;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.ApplicationInsights;

namespace GarageGroup.Internal.Support;

internal sealed partial class IncidentCreateCommand : IIncidentCreateCommand
{
    private const string SystemUserIdClaim = "SystemUserId";

    private readonly ICrmIncidentApi incidentApi;

    private readonly ICrmOwnerApi ownerApi;

    private readonly ISupportGptApi gptApi;

    private readonly ICrmProjectApi projectApi;

    private readonly TelemetryClient? telemetryClient;

    private readonly IncidentCreateFlowOption option;

    internal IncidentCreateCommand(
        ICrmIncidentApi incidentApi,
        ICrmOwnerApi ownerApi,
        ICrmProjectApi projectApi,
        ISupportGptApi gptApi,
        TelemetryClient? telemetryClient,
        IncidentCreateFlowOption option)
    {
        this.incidentApi = incidentApi;
        this.ownerApi = ownerApi;
        this.projectApi = projectApi;
        this.gptApi = gptApi;
        this.telemetryClient = telemetryClient;
        this.option = option;
    }

    private static Guid ParseSystemUserId(ChatUser user)
    {
        var systemIdClaim = user.Identity?.Claims.AsEnumerable().FirstOrDefault(IsSystemUserIdClaim).Value;
        if (string.IsNullOrWhiteSpace(systemIdClaim))
        {
            throw new InvalidOperationException($"User claim {SystemUserIdClaim} is not found.");
        }

        if (Guid.TryParse(systemIdClaim, out var userId) is false)
        {
            throw new InvalidOperationException($"User claim {SystemUserIdClaim} value '{systemIdClaim}' is invalid.");
        }

        return userId;

        static bool IsSystemUserIdClaim(KeyValuePair<string, string> claim)
            =>
            string.Equals(SystemUserIdClaim, claim.Key, StringComparison.InvariantCultureIgnoreCase);
    }
}