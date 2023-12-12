using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    [HttpFunction("HealthCheck", HttpMethodName.Get, Route = "health", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IHealthCheckHandler> UseHealthCheck()
        =>
        HealthCheck.UseServices(
            UseCosmosStorage().UseServiceHealthCheckApi("CosmosStorage"),
            UseSqlApi().UseServiceHealthCheckApi("DataverseDb"),
            UseDataverseApi().UseServiceHealthCheckApi("DataverseApi"))
        .UseHealthCheckHandler();
}