using System;

namespace GarageGroup.Internal.Support;

public sealed record class ContactGetIn
{
    public ContactGetIn(string telegramSenderId)
        =>
        TelegramSenderId = telegramSenderId.OrEmpty();

    public string TelegramSenderId { get; }
}