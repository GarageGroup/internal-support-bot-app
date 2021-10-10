using System;
using System.Collections.Generic;

namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    private static Type EqualityContract
        =>
        typeof(ChatFlowStepResult<TFlowState>);

    private static IEqualityComparer<TFlowState> FlowStateComparer
        =>
        EqualityComparer<TFlowState>.Default;

    private static IEqualityComparer<ChatFlowStepResultCode> CodeComparer
        =>
        EqualityComparer<ChatFlowStepResultCode>.Default;
}
