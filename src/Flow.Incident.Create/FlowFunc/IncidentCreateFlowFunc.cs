using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Bot;

using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

internal sealed partial class IncidentCreateFlowFunc : IIncidentCreateFlowFunc
{
    internal static IncidentCreateFlowFunc InternalCreate(
        IIncidentCreateFunc incidentCreateFunc,
        IIncidentCreateFlowConfiguration flowConfiguration,
        ILogger<IncidentCreateFlowFunc> logger)
        =>
        new(incidentCreateFunc, flowConfiguration, logger);

    private readonly IIncidentCreateFunc incidentCreateFunc;

    private readonly IIncidentCreateFlowConfiguration flowConfiguration;

    private readonly ILogger logger;

    private IncidentCreateFlowFunc(
        IIncidentCreateFunc incidentCreateFunc,
        IIncidentCreateFlowConfiguration flowConfiguration,
        ILogger<IncidentCreateFlowFunc> logger)
    {
        this.incidentCreateFunc = incidentCreateFunc;
        this.flowConfiguration = flowConfiguration;
        this.logger = logger;
    }
}
