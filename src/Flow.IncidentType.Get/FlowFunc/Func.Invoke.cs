using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

partial class IncidentTypeGetFlowFunc
{
    public ValueTask<ChatFlowStepResult<IncidentTypeGetFlowOut>> InvokeAsync(
        DialogContext dialogContext, Unit _, CancellationToken cancellationToken = default)
        =>
        ChatFlow.Start(
            dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)))
        .MapFlowState(
            @in => typeCodes)
        .SendActivity(
            dialogContext.Context.Activity.CreateTypeHintActivity)
        .Await()
        .ForwardValue(
            CheckTitleResponseAsync)
        .CompleteValueAsync(
            cancellationToken);



    private static ValueTask<ChatFlowStepResult<IncidentTypeGetFlowOut>> CheckTitleResponseAsync(
        DialogContext dialogContext, IReadOnlyDictionary<int, string> caseCodeTypes, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe<IReadOnlyCollection<int>>(
            caseCodeTypes.Keys.ToArray(),
            cancellationToken)
        .Pipe(
            dialogContext.Context.Activity.GetTypeValueOrFailure)
        .MapFailure(
            async (failure, token) =>
            {
                if (failure.FailureMessage.IsNullOrEmpty())
                {
                    return default;
                }
                var failureActivity = MessageFactory.Text(failure.FailureMessage);
                await dialogContext.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                return default(Unit);
            })
        .Fold(
            caseCode => new IncidentTypeGetFlowOut(caseTypeCode: caseCode),
            ChatFlowStepResult.RetryAndAwait<IncidentTypeGetFlowOut>);
}
