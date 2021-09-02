namespace GGroupp.Internal.Support.Bot;

public sealed record UserLogInFlowOut
{
    public UserLogInFlowOut(Guid userId) => UserId = userId;

    public Guid UserId { get; }
}
