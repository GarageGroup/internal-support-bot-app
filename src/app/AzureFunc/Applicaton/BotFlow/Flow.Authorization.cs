using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class Application
{
    internal static BotBuilder UseAuthorization(this BotBuilder builder)
        =>
        builder.Next(
            UseUserAuthorizationApi().UseAuthorizationMiddleware(BotAuthorizationSectionName));
}