using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
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
        AsyncPipeline.Pipe(
            dialogContext.Context.Activity, cancellationToken)
        .Pipe(
            CustomerChooseActivity.GetCustomerOrAbsent)
        .MapFailureValue(
            (_, token) => ShowCustomerSetAsync(dialogContext, token))
        .Fold(
            ChatFlowStepResult.Next,
            Pipeline.Pipe);

    private ValueTask<ChatFlowStepResult<IncidentCustomerFindFlowOut>> ShowCustomerSetAsync(
        DialogContext dialogContext, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            dialogContext.Context.Activity.Text.OrEmpty(),
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
                var activity = dialogContext.Context.Activity.CreateCustomerChooseActivity(customers);
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
        .Fold(
            ChatFlowStepResult.RetryAndAwait<IncidentCustomerFindFlowOut>,
            failureCode => failureCode);

    private Failure<ChatFlowStepAlternativeCode> MapCustomerSetFindFailure(Failure<CustomerSetFindFailureCode> failure)
    {
        logger.LogError(failure.FailureMessage);
        return Failure.Create(ChatFlowStepAlternativeCode.Interruption, UnexpectedFailureMessage);
    }

    private const int MinSearchTextLength = 3;

    private const string UnexpectedFailureMessage
        =
        "При выполнении запроса произошла непредвиденная ошибка. Обратитесь к администратору и повторите попытку позднее";
}
