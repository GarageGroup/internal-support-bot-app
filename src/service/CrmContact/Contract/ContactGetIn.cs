namespace GarageGroup.Internal.Support;

public readonly record struct ContactGetIn
{
    public ContactGetIn(long telegramSenderId)
        =>
        TelegramSenderId = telegramSenderId;

    public long TelegramSenderId { get; }
}