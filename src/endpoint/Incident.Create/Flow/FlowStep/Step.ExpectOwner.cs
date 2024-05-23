using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectOwnerOrSkip(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmOwnerApi crmOwnerApi)
        =>
        chatFlow.ExpectChoiceValueOrSkip(
            crmOwnerApi.CreateOwnerStepOption);

    private static ChoiceStepOption<IncidentCreateFlowState, IncidentOwnerState>? CreateOwnerStepOption(
        this ICrmOwnerApi crmOwnerApi, IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.Owner is not null)
        {
            return null;
        }

        return new(
            choiceSetFactory: GetOwnersAsync,
            resultMessageFactory: CreateResultMessage,
            selectedItemMapper: MapFlowState);

        ValueTask<Result<ChoiceStepSet<IncidentOwnerState>, ChatRepeatState>> GetOwnersAsync(
            ChoiceStepRequest request, CancellationToken cancellationToken)
            =>
            string.IsNullOrEmpty(request.Text) switch
            {
                true => crmOwnerApi.GetDefaultOwnersAsync(context, cancellationToken),
                _ => crmOwnerApi.SearchOwnersAsync(context, request, cancellationToken)
            };

        ChatMessageSendRequest CreateResultMessage(ChoiceStepItem<IncidentOwnerState> item)
            =>
            new(context.BuildOwnerResultMessage(item.Title))
            {
                ReplyMarkup = new BotReplyKeyboardRemove()
            };

        IncidentCreateFlowState MapFlowState(ChoiceStepItem<IncidentOwnerState> item)
            =>
            context.FlowState with
            {
                Owner = item.Value
            };
    }

    private static ValueTask<Result<ChoiceStepSet<IncidentOwnerState>, ChatRepeatState>> GetDefaultOwnersAsync(
        this ICrmOwnerApi crmOwnerApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static state => new LastOwnerSetGetIn(
                customerId: state.Customer?.Id ?? default,
                userId: state.BotUserId,
                top: MaxOwnerSetCount - 1))
        .PipeValue(
            crmOwnerApi.GetLastAsync)
        .OnFailure(
            failure => context.Logger.LogError(
                "Get last owners failure: {failureCode} {failureMessage}", failure.FailureCode, failure.FailureMessage))
        .Fold<FlatArray<ChoiceStepItem<IncidentOwnerState>>, ChatRepeatState>(
            success => success.Owners.Map(context.MapOwner),
            static _ => default(FlatArray<ChoiceStepItem<IncidentOwnerState>>))
        .MapSuccess(
            owners => new ChoiceStepSet<IncidentOwnerState>(
                choiceText: context.Localizer[ChooseOrSearchOwnerText])
            {
                Items = owners.InsertBotUser(context.FlowState)
            });

    private static ValueTask<Result<ChoiceStepSet<IncidentOwnerState>, ChatRepeatState>> SearchOwnersAsync(
        this ICrmOwnerApi crmOwnerApi, IChatFlowContextBase context, ChoiceStepRequest request, CancellationToken token)
        =>
        AsyncPipeline.Pipe(
            request.Text, token)
        .Pipe(
            static text => new OwnerSetSearchIn(text)
            {
                Top = MaxOwnerSetCount
            })
        .PipeValue(
            crmOwnerApi.SearchAsync)
        .MapFailure(
            context.MapFailure)
        .MapSuccess(
            @out => new ChoiceStepSet<IncidentOwnerState>(
                choiceText: @out.Owners.IsEmpty ? context.Localizer[NotFoundOwnersText] : context.Localizer[ChooseOrSearchOwnerText])
            {
                Items = @out.Owners.Map(context.MapOwner)
            });

    private static FlatArray<ChoiceStepItem<IncidentOwnerState>> InsertBotUser(
        this FlatArray<ChoiceStepItem<IncidentOwnerState>> values, IncidentCreateFlowState flowState)
    {
        var builder = FlatArray<ChoiceStepItem<IncidentOwnerState>>.Builder.OfLength(values.Length + 1);

        builder[0] = new(
            id: flowState.BotUserId.ToString(),
            title: flowState.BotUserName.OrEmpty(),
            value: new(flowState.BotUserId, flowState.BotUserName));

        for (var i = 0; i < values.Length; i++)
        {
            builder[i + 1] = values[i];
        }

        return builder.MoveToFlatArray();
    }

    private static ChoiceStepItem<IncidentOwnerState> MapOwner(this IChatFlowContextBase context, OwnerItemOut item)
        =>
        new(
            id: item.Id.ToString(),
            title: item.FullName,
            value: new(item.Id, item.FullName));

    private static ChatRepeatState MapFailure(this IChatFlowContextBase context, Failure<OwnerSetGetFailureCode> failure)
        =>
        failure.ToChatRepeatState(
            userMessage: failure.FailureCode switch
            {
                OwnerSetGetFailureCode.NotAllowed => context.Localizer[NotAllowedText],
                _ => throw failure.ToException()
            });
}