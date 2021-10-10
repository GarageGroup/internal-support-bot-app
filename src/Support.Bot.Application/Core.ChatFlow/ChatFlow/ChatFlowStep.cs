using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

public delegate ValueTask<ChatFlowStepResult<TResultFlowState>> ChatFlowStep<TFlowState, TResultFlowState>(
    DialogContext context, TFlowState flowState, CancellationToken cancellationToken = default);
