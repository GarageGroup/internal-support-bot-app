using System;

namespace GGroupp.Infra;

public readonly partial struct ChatFlowStepResult<TFlowState> : IEquatable<ChatFlowStepResult<TFlowState>>
{
    private readonly TFlowState flowState;

    public ChatFlowStepResultCode Code { get; }

    private ChatFlowStepResult(TFlowState flowState)
    {
        Code = ChatFlowStepResultCode.Next;
        this.flowState = flowState;
    }

    private ChatFlowStepResult(ChatFlowStepResultCode code)
    {
        Code = code;
        flowState = default!;
    }
}
