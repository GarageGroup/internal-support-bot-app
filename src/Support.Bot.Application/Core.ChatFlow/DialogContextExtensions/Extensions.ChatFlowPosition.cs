using System;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

internal static partial class DialogContextExtensions
{
    public static int GetChatFlowPosition(this DialogContext dialogContext, int flowLevel)
    {
        var positionParamName = GetChatFlowPositionParamName(flowLevel);

        if (dialogContext.ActiveDialog.State.TryGetValue(positionParamName, out var position))
        {
            if (position is not null)
            {
                return (int)position;
            }
        }

        return -1;
    }

    public static Unit SetChatFlowPosition(this DialogContext dialogContext, int flowLevel, int position)
    {
        var positionParamName = GetChatFlowPositionParamName(flowLevel);
        dialogContext.ActiveDialog.State[positionParamName] = position;

        return default;
    }
}
