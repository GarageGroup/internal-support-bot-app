using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Bot;

partial class UserLogInGetFlowFunc
{
    public ValueTask<ChatFlowStepResult<UserLogInFlowOut>> InvokeAsync(
        DialogContext dialogContext, Unit _, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)),
            cancellationToken)
        .Pipe(
            (context, token) => userDataAccessor.GetAsync(context.Context, () => default, token))
        .Pipe(
            userData => Optional.Present(userData).FilterNotNull().ToResult())
        .FoldValue(
            (userData, _) => ValueTask.FromResult<ChatFlowStepResult<UserFlowStateJson>>(userData),
            (_, token) => LogInAsync(dialogContext, token))
        .Pipe(
            result => result.MapFlowState(MapFlowState));

    private ValueTask<ChatFlowStepResult<UserFlowStateJson>> LogInAsync(
        DialogContext context, CancellationToken cancellationToken)
        =>
        ChatFlow.Start(
            context)
        .ForwardValue(
            GetUserActiveDirectoryIdAsync)
        .ForwardValue(
            GetUserDataAsync)
        .On(
            (flowState, token) => userDataAccessor.SetAsync(context.Context, flowState, token))
        .CompleteValueAsync(
            cancellationToken);

    private async ValueTask<ChatFlowStepResult<Guid>> GetUserActiveDirectoryIdAsync(
        DialogContext context, Unit _, CancellationToken cancellationToken)
    {
        if (context.Context.Activity.ChannelId == Channels.Msteams)
        {
            var member = await TeamsInfo.GetMemberAsync(context.Context, context.Context.Activity.From.Id, cancellationToken).ConfigureAwait(false);
            if (member is not null)
            {
                return Guid.Parse(member.AadObjectId);
            }
        }

        return await LogInActiveDirectoryAsync(context, cancellationToken).ConfigureAwait(false);
    }

    private ValueTask<ChatFlowStepResult<Guid>> LogInActiveDirectoryAsync(
        DialogContext context, CancellationToken cancellationToken)
        =>
        ChatFlow.Start(
            context, "Войдите в свою учетную запись")
        .MapFlowState(
            GetOAuthSettings)
        .On(
            (settings, token) => OAuthPrompt.SendOAuthCardAsync(settings, context.Context, null, token))
        .Await()
        .ForwardValue(
            async (settings, token) =>
            {
                var result = await OAuthPrompt.RecognizeTokenAsync(settings, context, token).ConfigureAwait(false);
                if (result.Succeeded is false)
                {
                    var retrySettings = GetOAuthSettings("Не удалось авторизавоться. Повторите попытку");
                    await OAuthPrompt.SendOAuthCardAsync(retrySettings, context.Context, null, token).ConfigureAwait(false);

                    return ChatFlowStepResult.RetryAndAwait<string>();
                }

                return result.Value.Token ?? string.Empty;
            })
        .ForwardValue(
            AuthorizeInActiveDirectoryAsync)
        .CompleteValueAsync(
            cancellationToken);

    private ValueTask<ChatFlowStepResult<UserFlowStateJson>> GetUserDataAsync(
        DialogContext context, Guid activeDirectoryUserId, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            new UserGetIn(activeDirectoryUserId), cancellationToken)
        .PipeValue(
            userGetFunc.InvokeAsync)
        .MapFailureValue(
            (failure, token) => MapUserGetFailureAsync(context.Context, failure, token))
        .Fold(
            user => new UserFlowStateJson
            {
                UserId = user.SystemUserId
            },
            ChatFlowStepResult.Interrupt<UserFlowStateJson>);

    private ValueTask<ChatFlowStepResult<Guid>> AuthorizeInActiveDirectoryAsync(
        DialogContext context, string accessToken, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            new ADUserGetIn(accessToken: accessToken), cancellationToken)
        .PipeValue(
            adUserGetFunc.InvokeAsync)
        .MapFailure(
            failure => failure.MapFailureCode(_ => UserGetFailureCode.Unknown))
        .MapFailureValue(
            (failure, token) => MapUserGetFailureAsync(context.Context, failure, token))
        .Fold(
            adUser => adUser.Id,
            ChatFlowStepResult.Interrupt<Guid>);

    private OAuthPromptSettings GetOAuthSettings(string text)
        =>
        new()
        {
            ConnectionName = userLogInConfiguration.OAuthConnectionName,
            Text = text,
            Title = OAuthPromptTitle,
            Timeout = userLogInConfiguration.OAuthTimeoutMilliseconds
        };

    private static UserLogInFlowOut MapFlowState(UserFlowStateJson flowState)
        =>
        new(userId: flowState.UserId);

    private async ValueTask<Unit> MapUserGetFailureAsync(ITurnContext context, Failure<UserGetFailureCode> failure, CancellationToken token)
    {
        logger.LogError(failure.FailureMessage);

        var failureActivity = MessageFactory.Text(CreateFailureMessage(failure.FailureCode));
        _ = await context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

        return default;
    }

    private static string CreateFailureMessage(UserGetFailureCode failureCode)
        =>
        failureCode == UserGetFailureCode.NotFound
            ? "Пользователь не найден. Обратитесь к администратору и повторите попытку позже."
            : "Возникла непредвиденная ошибка при попытке получить данные пользователя. Возможно сервер временно не доступен. Повторите попытку позже";
}