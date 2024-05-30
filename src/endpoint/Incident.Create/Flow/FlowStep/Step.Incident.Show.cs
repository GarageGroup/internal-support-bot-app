using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Localization;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ShowIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IncidentCreateFlowOption option)
        =>
        chatFlow.On(
            option.SendSuccessMessageAsync);

    private static Task SendSuccessMessageAsync(
        this IncidentCreateFlowOption option, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        var text = option.BuildIncidentMessage(context);

        Task[] tasks =
        [
            context.Api.SendHtmlModeTextAndRemoveReplyKeyboardAsync(text, cancellationToken),
            context.Api.DeleteMessageAsync(context.FlowState.TemporaryMessageId, cancellationToken)
        ];

        return Task.WhenAll(tasks);
    }

    private static string BuildIncidentMessage(this IncidentCreateFlowOption option, IChatFlowContext<IncidentCreateFlowState> context)
    {
        var url = string.Format(CultureInfo.InvariantCulture, option.IncidentCardUrlTemplate, context.FlowState.IncidentId);

        var title = HttpUtility.HtmlEncode(context.FlowState.Title);
        var link = string.Format(CultureInfo.InvariantCulture, "<a href='{0}'>{1}</a>", url, title);

        if (context.FlowState.AnnotationFailureFileNames.IsEmpty)
        {
            return context.Localizer.GetString(IncidentCreationSuccessTemplate, link);
        }

        var builder = new StringBuilder(context.Localizer.GetString(IncidentCreationSuccessTemplate, link));

        var failureFileNames = string.Join(", ", context.FlowState.AnnotationFailureFileNames.AsEnumerable().Select(GetFileName));
        if (string.IsNullOrEmpty(failureFileNames) is false)
        {
            var text = context.Localizer.GetString(IncidentCreationAnnotationFailureTemplate, failureFileNames);
            builder.Append("\n\r\n\r").Append(text);
        }

        var invalidSizeFileNames = context.FlowState.AnnotationFailureFileNames.AsEnumerable().Select(GetInvalidSizeFileName);
        var failureFileNamesInvalidFileSize = string.Join(", ", invalidSizeFileNames);

        if (string.IsNullOrEmpty(failureFileNamesInvalidFileSize) is false)
        {
            var text = context.Localizer.GetString(IncidentCreationAnnotationFailureInvalidFileSizeTemplate, failureFileNamesInvalidFileSize);
            builder.Append("\n\r\n\r").Append(text);
        }

        return builder.ToString();

        static string? GetFileName(AnnotationFailureState failure)
        {
            if (failure.FailureCode is AnnotationCreateFailureCode.InvalidFileSize)
            {
                return null;
            }

            return string.Format(CultureInfo.InvariantCulture, "<b>{0}</b>", failure.FileName);
        }

        static string? GetInvalidSizeFileName(AnnotationFailureState failure)
        {
            if (failure.FailureCode is not AnnotationCreateFailureCode.InvalidFileSize)
            {
                return null;
            }

            return string.Format(CultureInfo.InvariantCulture, "<b>{0}</b>", failure.FileName);
        }
    }
}