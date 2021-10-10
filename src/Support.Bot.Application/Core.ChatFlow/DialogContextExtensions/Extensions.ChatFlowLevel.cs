using System;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

internal static partial class DialogContextExtensions
{
    public static int GetChatFlowLevel(this DialogContext dialogContext)
    {
        if (dialogContext.ActiveDialog.State.TryGetValue(ChatFlowLevelParamName, out var flowLevel))
        {
            if (flowLevel is not null)
            {
                return (int)flowLevel;
            }
        }

        return -1;
    }

    public static Unit SetChatFlowLevel(this DialogContext dialogContext, int flowLevel)
    {
        dialogContext.ActiveDialog.State[ChatFlowLevelParamName] = flowLevel;
        return default;
    }

    public static Unit ClearChatFlowLevel(this DialogContext dialogContext)
    {
        dialogContext.ActiveDialog.State[ChatFlowLevelParamName] = null;
        return default;
    }

    public static Unit ClearChatFlowLevelResources(this DialogContext dialogContext, int flowLevel)
    {
        var positionParamName = GetChatFlowPositionParamName(flowLevel);
        dialogContext.ActiveDialog.State[positionParamName] = null;

        var stateParamName = GetChatFlowStateParamName(flowLevel);
        dialogContext.ActiveDialog.State[stateParamName] = null;

        return default;
    }
}
