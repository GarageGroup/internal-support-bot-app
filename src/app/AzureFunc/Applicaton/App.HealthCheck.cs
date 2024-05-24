using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    [HttpFunction("HealthCheck", HttpMethodName.Get, Route = "health", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IHealthCheckHandler> UseHealthCheck()
        =>
        HealthCheck.UseServices(
            Dependency.From(ResolveBotApi).UseServiceHealthCheckApi("TelegramBotApi"),
            UseSqlApi().UseServiceHealthCheckApi("DataverseDb"),
            UseDataverseApi().UseServiceHealthCheckApi("DataverseApi"))
        .UseHealthCheckHandler();
}