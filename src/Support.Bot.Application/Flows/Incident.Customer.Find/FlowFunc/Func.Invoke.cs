using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Bot;

partial class IncidentCustomerFindFlowFunc
{
    public ValueTask<ChatFlowStepResult<IncidentCustomerFindFlowOut>> InvokeAsync(
        DialogContext dialogContext, Unit _, CancellationToken cancellationToken = default)
        =>
        ChatFlow.Start(
            dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)))
        .SendActivity(
            _ => MessageFactory.Text("Нужно выбрать клиента. Введите часть названия для поиска"))
        .Await()
        .ForwardValue(
            FindCustomerAsync)
        .CompleteValueAsync(
            cancellationToken);

    private ValueTask<ChatFlowStepResult<IncidentCustomerFindFlowOut>> FindCustomerAsync(
        DialogContext dialogContext, Unit _, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            dialogContext.Context.Activity, cancellationToken)
        .Pipe(
            activity => activity.GetDeserializedValue<CustomerChooseValueJson>())
        .MapFailureValue(
            (_, token) => ShowCustomerSetAsync(dialogContext, token))
        .Fold(
            customer => new IncidentCustomerFindFlowOut(customer.Id, customer.Title),
            failure => failure);

    private ValueTask<ChatFlowStepResult<IncidentCustomerFindFlowOut>> ShowCustomerSetAsync(
        DialogContext dialogContext, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            dialogContext.Context.Activity.Text ?? string.Empty,
            cancellationToken)
        .Pipe<string, Failure<ChatFlowStepAlternativeCode>>(
            activityText => activityText switch
            {
                { Length: >= MinSearchTextLength } => activityText,
                _ => Failure.Create(ChatFlowStepAlternativeCode.RetryAndAwaiting, $"Введите больше символов, хотя бы {MinSearchTextLength}")
            })
        .MapSuccess(
            searchText => new CustomerSetFindIn(searchText))
        .ForwardValue(
            customerSetFindFunc.InvokeAsync,
            MapCustomerSetFindFailure)
        .Forward<IReadOnlyCollection<CustomerItemFindOut>>(
            customerSetResult => customerSetResult.Customers.Any() switch
            {
                true => Result.Success(customerSetResult.Customers),
                _ => Failure.Create(ChatFlowStepAlternativeCode.RetryAndAwaiting, "Не удалось найти ни одного клиента. Попробуйте уточнить запрос")
            })
        .MapSuccess(
            async (customers, token) =>
            {
                var activity = customers.Take(MaxCustomerSetCount).Pipe(dialogContext.Context.CreateCustomerChooseActivity);
                await dialogContext.Context.SendActivityAsync(activity, token).ConfigureAwait(false);

                return default(Unit);
            })
        .MapFailure(
            async (failure, token) =>
            {
                var failureActivity = MessageFactory.Text(failure.FailureMessage);
                await dialogContext.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                return failure.FailureCode;
            })
        .Fold<ChatFlowStepResult<IncidentCustomerFindFlowOut>>(
            _ => ChatFlowStepResult.RetryAndAwait(),
            failureCode => failureCode);

    private Failure<ChatFlowStepAlternativeCode> MapCustomerSetFindFailure(Failure<CustomerSetFindFailureCode> failure)
    {
        logger.LogError(failure.FailureMessage);
        return Failure.Create(ChatFlowStepAlternativeCode.Interruption, UnexpectedFailureMessage);
    }
}
