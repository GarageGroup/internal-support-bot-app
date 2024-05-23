using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static BotCommandBuilder WithBotStopCommand(this BotCommandBuilder builder)
        =>
        builder.With(
            "stop", BotCommand.UseStopCommand());
}