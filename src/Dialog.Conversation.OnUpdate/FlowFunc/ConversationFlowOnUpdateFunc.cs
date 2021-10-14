using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

using IConversationFlowOnUpdateFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<Unit>>;

internal sealed partial class ConversationFlowOnUpdateFunc : IConversationFlowOnUpdateFunc
{
    internal static ConversationFlowOnUpdateFunc InternalCreate() => new();

    private ConversationFlowOnUpdateFunc()
    {
    }
}
