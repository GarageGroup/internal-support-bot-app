using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static BotCommandBuilder WithBotMenuCommand(this BotCommandBuilder builder)
        =>
        builder.With(
            BotCommand.UseMenuCommand());
}