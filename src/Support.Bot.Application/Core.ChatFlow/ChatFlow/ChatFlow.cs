using System;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

public static class ChatFlow
{
    public static ChatFlow<TFlowState> Start<TFlowState>(DialogContext dialogContext, TFlowState flowState)
        =>
        ChatFlow<TFlowState>.InternalStart(
            dialogContext: dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)),
            flowState: flowState);

    public static ChatFlow<Unit> Start(DialogContext dialogContext)
        =>
        ChatFlow<Unit>.InternalStart(
            dialogContext: dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)),
            flowState: default);
}
