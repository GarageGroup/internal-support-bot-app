namespace GarageGroup.Internal.Support;

public sealed record class TelegramSender
{
    public TelegramSender(long id)
        =>
        Id = id;

    public long Id { get; }
}