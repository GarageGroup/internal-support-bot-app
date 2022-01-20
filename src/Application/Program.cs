using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GGroupp.Internal.Support;

static class Program
{
    static async Task Main(string[] args)
        =>
        await RunAsync(args);

    private static Task RunAsync(string[] args)
        =>
        Host.CreateDefaultBuilder(args)
        .ConfigureSocketsHttpHandlerProvider()
        .ConfigureBotBuilder(
            GSupportBotBuilder.ResolveCosmosStorage)
        .ConfigureBotWebHostDefaults(
            ConfigureGSupportBot)
        .Build()
        .RunAsync();

    private static IBotBuilder ConfigureGSupportBot(IBotBuilder bot)
        =>
        bot.UseLogout("logout")
        .UseGSupportBotStop("stop")
        .UseGSupportAuthorization()
        .UseGSupportBotInfo("info")
        .UseGSupportIncidentCreate();
}