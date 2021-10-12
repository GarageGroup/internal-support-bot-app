using System;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

using IIncidentTitleGetFlowFunc = IAsyncValueFunc<DialogContext, IncidentTitleGetFlowIn, ChatFlowStepResult<IncidentTitleGetFlowOut>>;

internal sealed partial class IncidentTitleGetFlowFunc : IIncidentTitleGetFlowFunc
{
    public static IncidentTitleGetFlowFunc Create(Unit _) => instance;

    private static readonly IncidentTitleGetFlowFunc instance;

    static IncidentTitleGetFlowFunc() => instance = new();

    private const int DefaultTitleLength = 30;

    private IncidentTitleGetFlowFunc()
    {
    }
}
