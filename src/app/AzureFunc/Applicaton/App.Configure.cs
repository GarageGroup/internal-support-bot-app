using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    internal static void Configure(IFunctionsWorkerApplicationBuilder builder)
        =>
        builder.Services.RegisterCosmosStorage().RegisterDataverseApi().RegisterSqlApi();

    private static IServiceCollection RegisterCosmosStorage(this IServiceCollection services)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("CosmosStorage")
        .UseTokenCredentialResource()
        .UsePollyStandard()
        .UseCosmosStorage("CosmosDb")
        .ToRegistrar(services)
        .RegisterScoped();

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