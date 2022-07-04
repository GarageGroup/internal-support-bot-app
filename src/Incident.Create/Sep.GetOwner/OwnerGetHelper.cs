using GGroupp.Infra.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

internal static class OwnerGetHelper
{
    internal static ValueTask<ChatFlowJump<LookupValue>> GetOwnerValueOrBreakAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        BotUserIdGetHelper.GetBotUserValueOrBreakAsync(context, cancellationToken);
}