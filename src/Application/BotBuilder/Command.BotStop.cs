using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class GSupportBotBuilder
{
    internal static IBotBuilder UseGSupportBotStop(this IBotBuilder botBuilder, string commandName)
        =>
        botBuilder.UseBotStop(commandName, static () => new(successText: "Операция остановлена"));
}
