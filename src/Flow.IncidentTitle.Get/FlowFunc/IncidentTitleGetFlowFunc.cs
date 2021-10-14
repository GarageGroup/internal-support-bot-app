using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

using IIncidentTitleGetFlowFunc = IAsyncValueFunc<DialogContext, IncidentTitleGetFlowIn, ChatFlowStepResult<IncidentTitleGetFlowOut>>;

internal sealed partial class IncidentTitleGetFlowFunc : IIncidentTitleGetFlowFunc
{
    public static IncidentTitleGetFlowFunc Instance { get; }

    static IncidentTitleGetFlowFunc() => Instance = new();

    private const int DefaultTitleLength = 30;

    private IncidentTitleGetFlowFunc()
    {
    }
}
