using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

internal static class OwnerAwaitHelper
{
    private const string ChooseOwnerMessage = "Выберите ответственного или введите часть имени для поиска";

    private const string UnsuccessfulSearchResultText = "Не удалось найти ни одного пользователя. Попробуйте уточнить запрос";

    private const int MaxUserSetCount = 6;

    internal static ValueTask<LookupValueSetOption> GetDefaultOwnersAsync(
        this ICrmOwnerApi crmOwnerApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static state => new LastOwnerSetGetIn(
                customerId: state.CustomerId,
                userId: state.BotUserId.GetValueOrDefault(),
                top: MaxUserSetCount - 1))
        .PipeValue(
            crmOwnerApi.GetLastAsync)
        .OnFailure(
            failure => context.Logger.LogError(
                "Get last owners failure: {failureCode} {failureMessage}", failure.FailureCode, failure.FailureMessage))
        .Fold(
            static success => success.Owners.Map(MapOwnerItem),
            static _ => default)
        .Pipe(
            owners => new LookupValueSetOption(
                items: owners.InsertBotUser(context.FlowState),
                choiceText: ChooseOwnerMessage));

    internal static ValueTask<Result<LookupValueSetOption, BotFlowFailure>> SearchUsersOrFailureAsync(
        this ICrmOwnerApi crmOwnerApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        string seachText,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .HandleCancellation()
        .Pipe(
            flowState => new OwnerSetSearchIn(seachText)
            {
                Top = MaxUserSetCount
            })
        .PipeValue(
            crmOwnerApi.SearchAsync)
        .MapFailure(
            MapToFlowFailure)
        .MapSuccess(
            static @out => new LookupValueSetOption(
                items: @out.Owners.Map(MapOwnerItem),
                choiceText: @out.Owners.IsNotEmpty ? ChooseOwnerMessage : UnsuccessfulSearchResultText));

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue userValue)
        =>
        $"Ответственный: {context.EncodeHtmlTextWithStyle(userValue.Name, BotTextStyle.Bold)}";

    private static LookupValue MapOwnerItem(OwnerItemOut item)
        =>
        new(item.Id, item.FullName);

    private static FlatArray<LookupValue> InsertBotUser(this FlatArray<LookupValue> values, IncidentCreateFlowState flowState)
    {
        var builder = FlatArray<LookupValue>.Builder.OfLength(values.Length + 1);

        builder[0] = new(
            id: flowState.BotUserId.GetValueOrDefault(),
            name: flowState.BotUserName?.OrNullIfWhiteSpace() ?? "Я (по умолчанию)");

        for (var i = 0; i < values.Length; i++)
        {
            builder[i + 1] = values[i];
        }

        return builder.MoveToFlatArray();
    }

    private static BotFlowFailure MapToFlowFailure(Failure<OwnerSetGetFailureCode> failure)
        =>
        (failure.FailureCode switch
        {
            OwnerSetGetFailureCode.NotAllowed
                => "При поиске пользователей произошла ошибка. У вашей учетной записи не достаточно разрешений. Обратитесь к администратору приложения",
            OwnerSetGetFailureCode.TooManyRequests
                => "Слишком много обращений к сервису. Попробуйте повторить попытку через несколько секунд",
            _
                => "При поиске пользователей произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее"
        })
        .Pipe(
            message => BotFlowFailure.From(message, failure.FailureMessage));
}