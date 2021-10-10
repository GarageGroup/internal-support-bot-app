using System;

namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public TFlowState FlowStateOrThrow()
        =>
        InnerFlowStateOrThrow(
            CreateExpectedPresentFlowStateException);

    public TFlowState FlowStateOrThrow(Func<Exception> exceptionFactory)
        =>
        InnerFlowStateOrThrow(
            exceptionFactory ?? throw new ArgumentNullException(nameof(exceptionFactory)));

    private TFlowState InnerFlowStateOrThrow(Func<Exception> exceptionFactory)
        =>
        Code == ChatFlowStepResultCode.Next ? flowState : throw exceptionFactory.Invoke();

    private static InvalidOperationException CreateExpectedPresentFlowStateException()
        =>
        new("The flow state result is expected to have a value.");
}
