using System;
using Microsoft.Bot.Schema;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TFlowState> SendActivity(Func<TFlowState, IActivity> activityFactory)
        =>
        InnerSendActivity(
            activityFactory ?? throw new ArgumentNullException(nameof(activityFactory)));

    private ChatFlow<TFlowState> InnerSendActivity(Func<TFlowState, IActivity> activityFactory)
        =>
        InnerOn(
            (flowState, token) => dialogContext.Context.SendActivityAsync(activityFactory.Invoke(flowState), token));
}
