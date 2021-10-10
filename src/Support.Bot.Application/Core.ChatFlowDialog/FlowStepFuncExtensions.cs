using System;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

public static class FlowStepFuncExtensions
{
    public static ChatFlowDialog<TFlowStateOut> ToChatFlowDialog<TFlowStateOut>(
        this IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<TFlowStateOut>> flowStep,
        string dialogId)
        =>
        InnerToChatFlowDialog(
            flowStep: flowStep ?? throw new ArgumentNullException(nameof(flowStep)),
            dialogId: dialogId);

    private static ChatFlowDialog<TFlowStateOut> InnerToChatFlowDialog<TFlowStateOut>(
        IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<TFlowStateOut>> flowStep,
        string dialogId)
        =>
        new(
            dialogId: dialogId,
            flowStep: flowStep.InvokeAsync);
}
