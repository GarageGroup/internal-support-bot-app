using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

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

        var builder = new StringBuilder(context.Localizer.GetString(IncidentCreationSuccessTemplate, link)).Append("\n\r\n\r");

        var failureFileNames = string.Join(", ", context.FlowState.AnnotationFailureFileNames.AsEnumerable().Select(GetFileName));
        var failureFileNamesInvalidFileSize = string.Join(", ", context.FlowState.AnnotationFailureFileNames.AsEnumerable().Select(GetFileNameInvalidFileSize));

        if (string.IsNullOrEmpty(failureFileNames) is false)
        {
            builder.Append(context.Localizer.GetString(IncidentCreationAnnotationFailureTemplate, failureFileNames)).Append("\n\r\n\r");
        }
        if (string.IsNullOrEmpty(failureFileNamesInvalidFileSize) is false)
        {
            builder.Append(context.Localizer.GetString(IncidentCreationAnnotationFailureInvalidFileSizeTemplate, failureFileNamesInvalidFileSize));
        }

        return builder.ToString();

        static string? GetFileName(AnnotationFailureState failure)
        {
            if (failure.FailureCode is not null)
            {
                return null;
            }

            return failure.FileName;
        }

        static string? GetFileNameInvalidFileSize(AnnotationFailureState failure)
        {
            if (failure.FailureCode is null)
            {
                return null;
            }

            return failure.FileName;
        }
    }
}