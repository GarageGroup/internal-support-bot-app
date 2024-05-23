namespace GarageGroup.Internal.Support;

internal sealed record class SourceSenderState
{
    public SourceSenderState(long userId)
        =>
        UserId = userId;

    public long UserId { get; }
}