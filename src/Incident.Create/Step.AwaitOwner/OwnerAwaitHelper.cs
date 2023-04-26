using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class OwnerAwaitHelper
{
    private const string DefaultChooseOwnerMessage = "Выберите себя или введите часть имени для поиска";

    private const string ChooseOwnerMessage = "Выберите ответственного или введите часть имени для поиска";

    private const string UnsuccessfulSearchResultText = "Не удалось найти ни одного пользователя. Попробуйте уточнить запрос";

    private const int MaxUserSetCount = 6;

    internal static ValueTask<LookupValueSetOption> GetDefaultOwnerAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static flowState => new LookupValueSetOption(
                items: new LookupValue(
                    id: flowState.BotUserId.GetValueOrDefault(),
                    name: flowState.BotUserName?.OrNullIfWhiteSpace() ?? "Я (по умолчанию)")
                    .AsFlatArray(),
                choiceText: DefaultChooseOwnerMessage));

    internal static ValueTask<Result<LookupValueSetOption, BotFlowFailure>> SearchUsersOrFailureAsync(
        this IUserSetSearchSupplier supportApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        string seachText,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .HandleCancellation()
        .Pipe(
            flowState => new UserSetSearchIn(seachText)
            {
                Top = MaxUserSetCount
            })
        .PipeValue(
            supportApi.SearchUserSetAsync)
        .MapFailure(
            MapToFlowFailure)
        .MapSuccess(
            static @out => new LookupValueSetOption(
                items: @out.Users.Map(MapUserItem),
                choiceText: @out.Users.IsNotEmpty ? ChooseOwnerMessage : UnsuccessfulSearchResultText));

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue userValue)
        =>
        $"Ответственный: {context.EncodeHtmlTextWithStyle(userValue.Name, BotTextStyle.Bold)}";

    private static LookupValue MapUserItem(UserItemSearchOut item)
        =>
        new(item.Id, item.FullName);

    private static BotFlowFailure MapToFlowFailure(Failure<UserSetSearchFailureCode> failure)
        =>
        (failure.FailureCode switch
        {
            UserSetSearchFailureCode.NotAllowed
                => "При поиске пользователей произошла ошибка. У вашей учетной записи не достаточно разрешений. Обратитесь к администратору приложения",
            UserSetSearchFailureCode.TooManyRequests
                => "Слишком много обращений к сервису. Попробуйте повторить попытку через несколько секунд",
            _
                => "При поиске пользователей произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее"
        })
        .Pipe(
            message => BotFlowFailure.From(message, failure.FailureMessage));
}