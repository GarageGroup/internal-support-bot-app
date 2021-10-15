using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IIncidentTitleGetFlowFunc = IAsyncValueFunc<DialogContext, IncidentTitleGetFlowIn, ChatFlowStepResult<IncidentTitleGetFlowOut>>;

public static class IncidentTitleGetFlowDependency
{
    public static Dependency<IIncidentTitleGetFlowFunc> UseIncidentTitleGetFlow()
        =>
        Dependency.Of<IIncidentTitleGetFlowFunc>(IncidentTitleGetFlowFunc.Instance);
}
