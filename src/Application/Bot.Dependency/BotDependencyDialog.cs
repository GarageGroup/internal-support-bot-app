using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

internal static class BotDependencyDialog
{
    public static Dependency<Dialog> UseConversationOnUpdateDialog()
        =>
        ConversationOnUpdateDialogDependency.UseConversationOnUpdateDialog();

    public static Dependency<Dialog> UseIncidentCreateDialog(this Dependency<UserState> userStateDependency)
        =>
        userStateDependency.UseUserLogInFlow()
        .With(
            BotDependencyFlow.UseIncidentTitleGetFlow())
        .With(
            BotDependencyFlow.UseIncidentCustomerFindFlow())
        .With(
            BotDependencyFlow.UseIncidentCreateFlow())
        .With(
            BotDependencyFlow.UseIncidentTypeGetFlow())
        .UseIncidentCreateDialog();
}
