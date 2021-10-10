using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Bot;

partial class UserLogInGetFlowFunc
{
    public ValueTask<ChatFlowStepResult<UserLogInFlowOut>> InvokeAsync(
        DialogContext dialogContext, Unit _, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)),
            cancellationToken)
        .Pipe(
            (context, token) => userDataAccessor.GetAsync(context.Context, () => default, token))
        .Pipe(
            userData => Optional.Present(userData).FilterNotNull().ToResult())
        .FoldValue(
            (userData, _) => ValueTask.FromResult<ChatFlowStepResult<UserFlowStateJson>>(userData),
            (_, token) => InnerLogInAsync(dialogContext, token))
        .Pipe(
            result => result.MapFlowState(MapFlowState));

    private ValueTask<ChatFlowStepResult<UserFlowStateJson>> InnerLogInAsync(
        DialogContext context, CancellationToken cancellationToken)
        =>
        ChatFlow.Start(
            context)
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
                    var retryActivity = MessageFactory.Text("Не удалось авторизавоться. Повторите попытку");
                    _ = await context.Context.SendActivityAsync(retryActivity, token).ConfigureAwait(false);

                    return ChatFlowStepResult.RetryAndAwait<string>();
                }

                return result.Value.Token ?? string.Empty;
            })
        .ForwardValue(
            AuthorizeInActiveDirectory)
        .On(
            (flowState, token) => userDataAccessor.SetAsync(context.Context, flowState, token))
        .CompleteValueAsync(
            cancellationToken);

    private ValueTask<ChatFlowStepResult<UserFlowStateJson>> AuthorizeInActiveDirectory(
        DialogContext context, string userToken, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            userToken, cancellationToken)
        .PipeValue(
            GetUserIdAsync)
        .MapFailure(
            async (failure, token) =>
            {
                logger.LogError(failure.FailureMessage);

                var failureActivity = MessageFactory.Text(CreateFailureMessage(failure.FailureCode));
                await context.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                return default(Unit);
            })
        .Fold<ChatFlowStepResult<UserFlowStateJson>>(
            user => new UserFlowStateJson
            {
                UserId = user.SystemUserId
            },
            _ => ChatFlowStepAlternativeCode.Interruption);

    private ValueTask<Result<UserGetOut, Failure<UserGetFailureCode>>> GetUserIdAsync(
        string accessToken, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            new ADUserGetIn(accessToken: accessToken), cancellationToken)
        .PipeValue(
            adUserGetFunc.InvokeAsync)
        .MapFailure(
            failure => failure.MapFailureCode(_ => UserGetFailureCode.Unknown))
        .MapSuccess(
            adUser => new UserGetIn(activeDirectoryUserId: adUser.Id))
        .ForwardValue(
            userGetFunc.InvokeAsync);

    private OAuthPromptSettings GetOAuthSettings(Unit _)
        =>
        new()
        {
            ConnectionName = userLogInConfiguration.OAuthConnectionName,
            Text = userLogInConfiguration.OAuthPromptText,
            Title = userLogInConfiguration.OAuthPromptTitle,
            Timeout = userLogInConfiguration.OAuthTimeoutMilliseconds
        };

    private static UserLogInFlowOut MapFlowState(UserFlowStateJson flowState)
        =>
        new(userId: flowState.UserId);

    private static string CreateFailureMessage(UserGetFailureCode failureCode)
        =>
        failureCode == UserGetFailureCode.NotFound
            ? "Пользователь не найден. Обратитесь к администратору и повторите попытку позже."
            : "Возникла непредвиденная ошибка при попытке получить данные пользователя. Возможно сервер временно не доступен. Повторите попытку позже";
}
