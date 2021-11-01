using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support.Bot;

internal sealed class DialogBot : ActivityHandler
{
    public static DialogBot Create(
        ConversationState conversationState, UserState userState, Dialog dialog, Dialog? onMembersAddedDialog = null)
        =>
        new(
            conversationState: conversationState ?? throw new ArgumentNullException(nameof(conversationState)),
            userState: userState ?? throw new ArgumentNullException(nameof(userState)),
            dialog: dialog ?? throw new ArgumentNullException(nameof(dialog)),
            onMembersAddedDialog: onMembersAddedDialog);

    private readonly BotState conversationState;

    private readonly BotState userState;

    private readonly Dialog dialog;

    private readonly Dialog? onMembersAddedDialog;

    private DialogBot(BotState conversationState, BotState userState, Dialog dialog, Dialog? onMembersAddedDialog)
    {
        this.conversationState = conversationState;
        this.userState = userState;
        this.dialog = dialog;
        this.onMembersAddedDialog = onMembersAddedDialog;
    }

    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
        await base.OnTurnAsync(turnContext, cancellationToken);

        // Save any state changes that might have occurred during the turn.
        await conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        await userState.SaveChangesAsync(turnContext, false, cancellationToken);
    }

    protected override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        =>
        dialog.RunAsync(turnContext, conversationState.CreateProperty<DialogState>(dialog.Id), cancellationToken);

    protected override async Task OnMembersAddedAsync(
        IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        await base.OnMembersAddedAsync(membersAdded, turnContext, cancellationToken);

        if (onMembersAddedDialog is not null)
        {
            await onMembersAddedDialog.RunAsync(turnContext, conversationState.CreateProperty<DialogState>(onMembersAddedDialog.Id), cancellationToken);
        }
    }
}
