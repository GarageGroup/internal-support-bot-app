using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

partial class ConversationFlowOnUpdateFunc
{
    public ValueTask<ChatFlowStepResult<Unit>> InvokeAsync(DialogContext dialogContext, Unit _, CancellationToken cancellationToken = default)
        =>
        ChatFlow.Start(
            dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)))
        .MapFlowState(
            _ => dialogContext.Context.Activity.MembersAdded.Where(m => m.Id != dialogContext.Context.Activity.Recipient.Id))
        .On(
            async (members, token) =>
            {
                foreach (var member in members)
                {
                    var reply = MessageFactory.Text("Привет! Это G-Support бот!");
                    await dialogContext.Context.SendActivityAsync(reply, cancellationToken).ConfigureAwait(false);
                }
            })
        .Forward(
            members => members.FirstOrDefault() is not null
                ? ChatFlowStepResult.Next(default(Unit))
                : ChatFlowStepResult.Interrupt())
        /*.ForwardChildValue(
            userLogInFlowGetFunc.InvokeAsync,
            (_, _) => default(Unit))*/
        .CompleteValueAsync(
            cancellationToken);
}
