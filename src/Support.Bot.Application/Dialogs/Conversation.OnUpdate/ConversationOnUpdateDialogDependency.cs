using System;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IUserLogInFlowGetFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;

public static class ConversationOnUpdateDialogDependency
{
    public static Dependency<Dialog> UseConversationOnUpdateDialog(this Dependency<IUserLogInFlowGetFunc> dependency)
        =>
        dependency.Map(CreateConversationOnUpdateDialog);

    private static Dialog CreateConversationOnUpdateDialog(IUserLogInFlowGetFunc userLogInFlowGetFunc)
        =>
        ConversationFlowOnUpdateFunc.InternalCreate(
            userLogInFlowGetFunc ?? throw new ArgumentNullException(nameof(userLogInFlowGetFunc)))
        .ToChatFlowDialog(
            "ConversationFlowOnUpdateDialog");
}
