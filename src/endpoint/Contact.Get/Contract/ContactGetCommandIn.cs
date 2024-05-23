using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

public sealed record class ContactGetCommandIn : IChatCommandIn<ContactGetCommandOut>
{
    public static string Type { get; } = "ContactGet";

    public ContactGetCommandIn(TelegramSender? telegramSender)
        =>
        TelegramSender = telegramSender;

    public ContactGetCommandIn(CommandCustomer? customer, CommandContact? contact)
    {
        Customer = customer;
        Contact = contact;
    }

    public TelegramSender? TelegramSender { get; }

    public CommandCustomer? Customer { get; }

    public CommandContact? Contact { get; }
}