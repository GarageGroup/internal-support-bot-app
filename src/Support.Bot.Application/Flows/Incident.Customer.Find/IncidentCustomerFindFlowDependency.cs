using System;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IIncidentCustomerFindFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<IncidentCustomerFindFlowOut>>;
using ICustomerSetFindFunc = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

public static class IncidentCustomerFindFlowDependency
{
    public static Dependency<IIncidentCustomerFindFlowFunc> UseIncidentCustomerFindFlow(
        this Dependency<ICustomerSetFindFunc, ILoggerFactory> dependency)
        =>
        dependency.Fold<IIncidentCustomerFindFlowFunc>(Create);

    private static IncidentCustomerFindFlowFunc Create(
        ICustomerSetFindFunc customerSetFindFunc, ILoggerFactory loggerFactory)
        =>
        InnerCreate(
            customerSetFindFunc ?? throw new ArgumentNullException(nameof(customerSetFindFunc)),
            loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)));

    private static IncidentCustomerFindFlowFunc InnerCreate(
        ICustomerSetFindFunc customerSetFindFunc, ILoggerFactory loggerFactory)
        =>
        IncidentCustomerFindFlowFunc.InternalCreate(
            customerSetFindFunc: customerSetFindFunc,
            logger: loggerFactory.CreateLogger<IncidentCustomerFindFlowFunc>());
}
