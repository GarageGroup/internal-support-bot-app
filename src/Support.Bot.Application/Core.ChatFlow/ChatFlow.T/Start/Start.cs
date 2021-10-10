using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public static ChatFlow<TFlowState> Start(DialogContext dialogContext, TFlowState flowState)
        =>
        InternalStart(
            dialogContext: dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)),
            flowState: flowState);

    internal static ChatFlow<TFlowState> InternalStart(DialogContext dialogContext, TFlowState flowState)
    {
        var flowLevel = dialogContext.GetChatFlowLevel() + 1; //Error! It must be in ForwardChildValue
        _ = dialogContext.SetChatFlowLevel(flowLevel);

        return new(
            flowLevel: flowLevel,
            flowPosition: default,
            dialogContext: dialogContext,
            _ => ValueTask.FromResult<ChatFlowStepResult<TFlowState>>(flowState));
    }
}
