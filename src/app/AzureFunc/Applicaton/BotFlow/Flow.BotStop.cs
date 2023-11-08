using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static IBotBuilder UseBotStopFlow(this IBotBuilder botBuilder)
        =>
        botBuilder.UseBotStop(StopCommand, static () => new(successText: "Операция остановлена"));
}