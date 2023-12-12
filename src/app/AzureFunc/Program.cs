using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GarageGroup.Internal.Support;

static class Program
{
    static Task Main()
        =>
        Host.CreateDefaultBuilder()
        .ConfigureFunctionsWorkerStandard(
            useHostConfiguration: false,
            configure: Application.Configure)
        .ConfigureBotBuilder(
            storageResolver: ServiceProviderServiceExtensions.GetRequiredService<ICosmosStorage>)
        .Build()
        .RunAsync();
}