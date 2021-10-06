using System;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

using IConversationFlowOnUpdateFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<Unit>>;
using IUserLogInFlowGetFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;

internal sealed partial class ConversationFlowOnUpdateFunc : IConversationFlowOnUpdateFunc
{
    internal static ConversationFlowOnUpdateFunc InternalCreate(IUserLogInFlowGetFunc userLogInFlowGetFunc)
        =>
        new(userLogInFlowGetFunc);

    private readonly IUserLogInFlowGetFunc userLogInFlowGetFunc;

    private ConversationFlowOnUpdateFunc(IUserLogInFlowGetFunc userLogInFlowGetFunc)
        =>
        this.userLogInFlowGetFunc = userLogInFlowGetFunc;
}
