using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
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
        var url = string.Format(CultureInfo.InvariantCulture, option.IncidentCardUrlTemplate, context.FlowState.IncidentId);
        var text = context.Localizer.GetString(IncidentCreationSuccessTemplate, url);

        Task[] tasks =
        [
            context.Api.SendHtmlModeTextAndRemoveReplyKeyboardAsync(text, cancellationToken),
            context.Api.DeleteMessageAsync(context.FlowState.TemporaryMessageId, cancellationToken)
        ];

        return Task.WhenAll(tasks);
    }
}