using System;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Bot;

using IIncidentCreateFlowFunc = IAsyncValueFunc<DialogContext, IncidentCreateFlowIn, ChatFlowStepResult<Unit>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

internal sealed partial class IncidentCreateFlowFunc : IIncidentCreateFlowFunc
{
    private const string Yes = "Да";

    private const string UnexpectedFailureMessage = "Не удалось создать инцидент. Возможно сервис не доступен. Обратитесь к администратору или повторите попытку позже";

    internal static IncidentCreateFlowFunc InternalCreate(IIncidentCreateFunc incidentCreateFunc, ILogger<IncidentCreateFlowFunc> logger)
        =>
        new(incidentCreateFunc, logger);

    private static bool IsYes(string text) => string.Equals(text, Yes, StringComparison.InvariantCultureIgnoreCase);

    private readonly IIncidentCreateFunc incidentCreateFunc;

    private readonly ILogger logger;

    private IncidentCreateFlowFunc(IIncidentCreateFunc incidentCreateFunc, ILogger<IncidentCreateFlowFunc> logger)
    {
        this.incidentCreateFunc = incidentCreateFunc;
        this.logger = logger;
    }
}
