using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GarageGroup.Internal.Support;

static class Program
{
    static Task Main()
        =>
        ApplicationHost.Create().Build().RunAsync();
}