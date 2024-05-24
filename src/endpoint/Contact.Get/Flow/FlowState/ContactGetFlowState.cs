using System;

namespace GarageGroup.Internal.Support;

internal sealed record class ContactGetFlowState
{
    public required Guid BotUserId { get; init; }

    public required DateTime DbMinDate { get; init; }

    public long? TelegramSenderId { get; init; }

    public CustomerState? Customer { get; init; }

    public ContactState? Contact { get; init; }
}