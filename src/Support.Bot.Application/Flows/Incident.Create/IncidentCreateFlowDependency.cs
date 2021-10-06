using System;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

public static class IncidentCreateFlowDependency
{
    public static Dependency<IIncidentCreateFlowFunc> UseIncidentCreateFlow(
        this Dependency<IIncidentCreateFunc, ILoggerFactory> dependency)
        =>
        dependency.Fold<IIncidentCreateFlowFunc>(Create);

    private static IncidentCreateFlowFunc Create(IIncidentCreateFunc incidentCreateFunc, ILoggerFactory loggerFactory)
        =>
        InnerCreate(
            incidentCreateFunc ?? throw new ArgumentNullException(nameof(incidentCreateFunc)),
            loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)));

    private static IncidentCreateFlowFunc InnerCreate(IIncidentCreateFunc incidentCreateFunc, ILoggerFactory loggerFactory)
        =>
        IncidentCreateFlowFunc.InternalCreate(
            incidentCreateFunc: incidentCreateFunc,
            logger: loggerFactory.CreateLogger<IncidentCreateFlowFunc>());
}
