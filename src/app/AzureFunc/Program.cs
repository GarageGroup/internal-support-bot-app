using System.Threading.Tasks;
using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GarageGroup.Internal.Support;

static class Program
{
    static Task Main()
        =>
        FunctionHost.CreateFunctionsWorkerBuilderStandard(
            useHostConfiguration: false,
            configure: Application.Configure)
        .ConfigureBotBuilder(
            storageResolver: ServiceProviderServiceExtensions.GetRequiredService<ICosmosStorage>)
        .Build()
        .RunAsync();
}