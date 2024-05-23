using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static BotCommandBuilder WithLogoutCommand(this BotCommandBuilder builder)
        =>
        builder.With(
            "logout", UseUserAuthorizationApi().UseSignOutCommand());
}