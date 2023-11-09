using System.Threading.Tasks;
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
        .ConfigureBotBuilder(Application.ResolveCosmosStorage)
        .Build()
        .RunAsync();
}