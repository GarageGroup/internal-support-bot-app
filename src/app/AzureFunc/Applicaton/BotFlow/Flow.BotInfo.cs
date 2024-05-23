using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static BotCommandBuilder WithBotInfoCommand(this BotCommandBuilder builder)
        =>
        builder.With(
            "info", BotCommand.UseBotInfoCommand());
}