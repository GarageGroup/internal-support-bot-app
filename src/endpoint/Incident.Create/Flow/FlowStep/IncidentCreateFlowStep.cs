using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

internal static partial class IncidentCreateFlowStep
{
    private const int MaxTitleLength = 200;

    private const int MaxOwnerSetCount = 6;

    private static readonly JsonSerializerOptions SerializerOptions
        =
        new(JsonSerializerDefaults.Web);

    private static readonly char[] WordSeparators
        =
        [ ' ', ',', '.', '!', '?' ];

    private static readonly PipelineParallelOption ParallelOption
        =
        new()
        {
            DegreeOfParallelism = 4
        };

    private static async Task<BotMessage> SendTemporaryMessageAsync(
        this IChatFlowContextBase context, string text, CancellationToken cancellationToken)
    {
        var request = new ChatMessageSendRequest(text)
        {
            DisableNotification = true,
            ReplyMarkup = new BotReplyKeyboardRemove()
        };

        var message = await context.Api.SendMessageAsync(request, cancellationToken).ConfigureAwait(false);
        await context.Api.SendChatActionAsync(BotChatAction.Typing, cancellationToken).ConfigureAwait(false);

        return message;
    }

    private static string GetDisplayName(this IChatFlowContextBase context, IncidentCaseTypeCode caseTypeCode)
        =>
        caseTypeCode switch
        {
            IncidentCaseTypeCode.Problem => context.Localizer[Problem],
            IncidentCaseTypeCode.Request => context.Localizer[Request],
            _ => context.Localizer[Question]
        };

    private static string GetDisplayName(this IChatFlowContextBase context, IncidentPriorityCode priorityCode)
        =>
        priorityCode switch
        {
            IncidentPriorityCode.High => context.Localizer[High],
            IncidentPriorityCode.Low => context.Localizer[Low],
            _ => context.Localizer[Normal]
        };

    private static string BuildOwnerResultMessage(this IChatFlowContextBase context, string ownerName)
        =>
        string.Format("{0}: <b>{1}</b>", context.Localizer[OwnerFieldName], HttpUtility.HtmlEncode(ownerName));
}