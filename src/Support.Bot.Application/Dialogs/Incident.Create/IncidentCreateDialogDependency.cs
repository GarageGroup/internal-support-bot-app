using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot
{
    using IUserLogInFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;
    using IIncidentTitleFlowFunc = IAsyncValueFunc<DialogContext, IncidentTitleGetFlowIn, ChatFlowStepResult<IncidentTitleGetFlowOut>>;
    using IIncidentCustomerFindFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<IncidentCustomerFindFlowOut>>;
    using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;

    public static class IncidentCreateDialogDependency
    {
        public static Dependency<Dialog> UseIncidentCreateDialog(
            this Dependency<IUserLogInFlowFunc, IIncidentTitleFlowFunc, IIncidentCustomerFindFlowFunc, IIncidentCreateFlowFunc> dependency)
            =>
            dependency.Fold(Create);

        private static Dialog Create(
            IUserLogInFlowFunc userLogInFlowFunc,
            IIncidentTitleFlowFunc incidentTitleFlowFunc,
            IIncidentCustomerFindFlowFunc incidentCustomerFindFlowFunc,
            IIncidentCreateFlowFunc incidentCreateFlowFunc)
            =>
            IncidentCreateDialogFlowFunc.InternalCreate(
                userLogInFlowFunc ?? throw new ArgumentNullException(nameof(userLogInFlowFunc)),
                incidentTitleFlowFunc ?? throw new ArgumentNullException(nameof(incidentTitleFlowFunc)),
                incidentCustomerFindFlowFunc ?? throw new ArgumentNullException(nameof(incidentCustomerFindFlowFunc)),
                incidentCreateFlowFunc ?? throw new ArgumentNullException(nameof(incidentCreateFlowFunc)))
            .ToChatFlowDialog(
                "IncidentCreate");
    }
}