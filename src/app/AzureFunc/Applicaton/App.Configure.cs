using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    internal static void Configure(IFunctionsWorkerApplicationBuilder builder)
        =>
        builder.Services.RegisterDataverseApi().RegisterSqlApi();

    private static IServiceCollection RegisterDataverseApi(this IServiceCollection services)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("DataverseApi")
        .UseTokenCredentialStandard()
        .UsePollyStandard()
        .UseDataverseApiClient(DataverseSectionName)
        .ToRegistrar(services)
        .RegisterScoped();

    private static IServiceCollection RegisterSqlApi(this IServiceCollection services)
        =>
        DataverseDbProvider.Configure(DataverseSectionName)
        .UseSqlApi()
        .ToRegistrar(services)
        .RegisterScoped();
}