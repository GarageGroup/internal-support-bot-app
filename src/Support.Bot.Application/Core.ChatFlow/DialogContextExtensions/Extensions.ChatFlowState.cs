using System;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

internal static partial class DialogContextExtensions
{
    public static TFlowState GetChatFlowState<TFlowState>(this DialogContext dialogContext, int flowLevel)
    {
        if (typeof(TFlowState) == typeof(Unit))
        {
            return default!;
        }

        var stateParamName = GetChatFlowStateParamName(flowLevel);

        if (dialogContext.ActiveDialog.State.TryGetValue(stateParamName, out var state))
        {
            if (state is TFlowState flowState)
            {
                return flowState;
            }

            throw CreateUnexpectedFlowStateTypeException<TFlowState>(state);
        }

        throw CreateStateNotFoundException(stateParamName);
    }

    public static Unit SetChatFlowState<TFlowState>(this DialogContext dialogContext, int flowLevel, TFlowState flowState)
    {
        var stateParamName = GetChatFlowStateParamName(flowLevel);
        dialogContext.ActiveDialog.State[stateParamName] = flowState;

        return default;
    }

    private static InvalidOperationException CreateUnexpectedFlowStateTypeException<TFlowState>(object? flowState)
        =>
        new($"A flow state was expected to be {typeof(TFlowState).Name} but it was {flowState?.GetType().Name ?? "null"}.");

    private static InvalidOperationException CreateStateNotFoundException(string stateParamName)
        =>
        new($"A flow state {stateParamName} was not found in the dialog state.");
}
