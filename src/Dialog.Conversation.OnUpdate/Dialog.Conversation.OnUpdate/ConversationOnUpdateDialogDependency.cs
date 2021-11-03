using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

public static class ConversationOnUpdateDialogDependency
{
    public static Dependency<Dialog> UseConversationOnUpdateDialog()
        =>
        Dependency.From(CreateConversationOnUpdateDialog);

    private static Dialog CreateConversationOnUpdateDialog()
        =>
        ChatFlowDialog.Create(
            "ConversationFlowOnUpdateDialog",
            ConversationFlowOnUpdateFunc.InternalCreate().InvokeAsync);
}
