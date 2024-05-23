using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncident
{
    internal static DbParameterFilter BuildFilter(long senderTelegramId)
        =>
        new($"{AliasName}.gg_sender_telegram_id", DbFilterOperator.Equal, senderTelegramId, "SenderTelegramId");
}