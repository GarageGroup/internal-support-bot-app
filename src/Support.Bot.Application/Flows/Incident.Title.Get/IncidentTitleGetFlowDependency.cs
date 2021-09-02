using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot
{
    using IIncidentTitleGetFlowFunc = IAsyncValueFunc<DialogContext, IncidentTitleGetFlowIn, ChatFlowStepResult<IncidentTitleGetFlowOut>>;

    public static class IncidentTitleGetFlowDependency
    {
        public static Dependency<IIncidentTitleGetFlowFunc> UseIncidentTitleGetFlow(
            this Dependency<Unit> dependency)
            =>
            dependency.Map<IIncidentTitleGetFlowFunc>(IncidentTitleGetFlowFunc.Create);
    }
}