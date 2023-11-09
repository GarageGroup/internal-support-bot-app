using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    internal static void Configure(IFunctionsWorkerApplicationBuilder builder)
        =>
        builder.Services.RegisterDataverseApi();

    private static IServiceCollection RegisterDataverseApi(this IServiceCollection services)
        =>
        UseHttpMessageHandlerStandard("DataverseApi")
        .UseDataverseApiClient(DataverseSectionName)
        .ToRegistrar(services)
        .RegisterScoped();
}