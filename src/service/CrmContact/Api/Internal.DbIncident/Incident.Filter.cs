using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncident
{
    internal static DbParameterFilter BuildFilter(string senderTelegramId)
        =>
        new($"{AliasName}.gg_sender_telegram_td", DbFilterOperator.Equal, senderTelegramId);
}