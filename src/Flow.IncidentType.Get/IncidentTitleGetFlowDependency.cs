using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IIncidentTypeGetFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<IncidentTypeGetFlowOut>>;

public static class IncidentTypeGetFlowDependency
{
    public static Dependency<IIncidentTypeGetFlowFunc> UseIncidentTypeGetFlow()
        =>
        Dependency.Of<IIncidentTypeGetFlowFunc>(IncidentTypeGetFlowFunc.Instance);
}
