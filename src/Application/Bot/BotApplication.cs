using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GGroupp.Internal.Support.Bot;

internal static class BotApplication
{
    public static Task RunAsync(string[] args)
        =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices(
            SocketsHttpHandlerProviderRegistrar.RegisterSingleton)
        .ConfigureWebHostDefaults(
            b => b.Configure(Configure))
        .Build()
        .RunAsync();

    private static void Configure(IApplicationBuilder app)
        =>
        app
        .UseWebSockets()
        .UseAuthorization(_ => new())
        .UseGSupportBot();
}
