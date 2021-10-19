using System;
using System.Collections.Generic;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

using IIncidentCreateDialogFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<Unit>>;
using IUserLogInFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;
using IIncidentTitleFlowFunc = IAsyncValueFunc<DialogContext, IncidentTitleGetFlowIn, ChatFlowStepResult<IncidentTitleGetFlowOut>>;
using IIncidentCustomerFindFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<IncidentCustomerFindFlowOut>>;
using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;
using IIncidentTypeGetFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<IncidentTypeGetFlowOut>>;

internal sealed partial class IncidentCreateDialogFlowFunc : IIncidentCreateDialogFlowFunc
{
    internal static IncidentCreateDialogFlowFunc InternalCreate(
        IUserLogInFlowFunc userLogInFlowFunc,
        IIncidentTitleFlowFunc incidentTitleFlowFunc,
        IIncidentCustomerFindFlowFunc incidentCustomerFindFlowFunc,
        IIncidentCreateFlowFunc incidentCreateFlowFunc,
        IIncidentTypeGetFlowFunc incidentTypeGetFlowFunc)
        =>
        new(
            userLogInFlowFunc,
            incidentTitleFlowFunc,
            incidentCustomerFindFlowFunc,
            incidentCreateFlowFunc,
            incidentTypeGetFlowFunc);

    private readonly IUserLogInFlowFunc userLogInFlowFunc;

    private readonly IIncidentTitleFlowFunc incidentTitleFlowFunc;

    private readonly IIncidentCustomerFindFlowFunc incidentCustomerFindFlowFunc;

    private readonly IIncidentCreateFlowFunc incidentCreateFlowFunc;

    private readonly IIncidentTypeGetFlowFunc incidentTypeGetFlowFunc;

    private IncidentCreateDialogFlowFunc(
        IUserLogInFlowFunc userLogInFlowFunc,
        IIncidentTitleFlowFunc incidentTitleFlowFunc,
        IIncidentCustomerFindFlowFunc incidentCustomerFindFlowFunc,
        IIncidentCreateFlowFunc incidentCreateFlowFunc,
        IIncidentTypeGetFlowFunc incidentTypeGetFlowFunc)
    {
        this.userLogInFlowFunc = userLogInFlowFunc;
        this.incidentTitleFlowFunc = incidentTitleFlowFunc;
        this.incidentCustomerFindFlowFunc = incidentCustomerFindFlowFunc;
        this.incidentCreateFlowFunc = incidentCreateFlowFunc;
        this.incidentTypeGetFlowFunc = incidentTypeGetFlowFunc;
    }
}
