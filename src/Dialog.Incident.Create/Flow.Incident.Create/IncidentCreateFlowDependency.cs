using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

public static class IncidentCreateFlowDependency
{
    public static Dependency<IIncidentCreateFlowFunc> UseIncidentCreateFlow<TFlowConfiguration>(
        this Dependency<IIncidentCreateFunc, TFlowConfiguration, ILoggerFactory> dependency)
        where TFlowConfiguration : IIncidentCreateFlowConfiguration
        =>
        dependency.Fold<IIncidentCreateFlowFunc>(Create);

    private static IncidentCreateFlowFunc Create<TFlowConfiguration>(
        IIncidentCreateFunc incidentCreateFunc,
        TFlowConfiguration flowConfiguration,
        ILoggerFactory loggerFactory)
        where TFlowConfiguration : IIncidentCreateFlowConfiguration
        =>
        InnerCreate(
            incidentCreateFunc ?? throw new ArgumentNullException(nameof(incidentCreateFunc)),
            flowConfiguration ?? throw new ArgumentNullException(nameof(flowConfiguration)),
            loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)));

    private static IncidentCreateFlowFunc InnerCreate(
        IIncidentCreateFunc incidentCreateFunc,
        IIncidentCreateFlowConfiguration flowConfiguration,
        ILoggerFactory loggerFactory)
        =>
        IncidentCreateFlowFunc.InternalCreate(
            incidentCreateFunc: incidentCreateFunc,
            flowConfiguration: flowConfiguration,
            logger: loggerFactory.CreateLogger<IncidentCreateFlowFunc>());
}
