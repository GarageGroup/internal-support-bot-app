using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot
{
    using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;
    using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

    internal sealed partial class IncidentCreateFlowFunc : IIncidentCreateFlowFunc
    {
        internal static IncidentCreateFlowFunc InternalCreate(IIncidentCreateFunc incidentCreateFunc, ILogger<IncidentCreateFlowFunc> logger)
            =>
            new(incidentCreateFunc, logger);

        private const string Yes = "Да";

        private readonly IIncidentCreateFunc incidentCreateFunc;

        private readonly ILogger logger;

        private IncidentCreateFlowFunc(IIncidentCreateFunc incidentCreateFunc, ILogger<IncidentCreateFlowFunc> logger)
        {
            this.incidentCreateFunc = incidentCreateFunc;
            this.logger = logger;
        }
    }
}