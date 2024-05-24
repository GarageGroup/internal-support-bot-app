using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

public interface IContactGetCommand : IChatCommand<ContactGetCommandIn, ContactGetCommandOut>;