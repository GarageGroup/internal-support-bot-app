using GarageGroup.Infra;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class ApplicationHost
{
    public static IHostBuilder CreateBuilder()
        =>
        FunctionHost.CreateFunctionsWorkerBuilderStandard(
            useHostConfiguration: false,
            configure: Configure);

    private static void Configure(IFunctionsWorkerApplicationBuilder builder)
        =>
        builder.Services.RegisterBlobBotStorage().RegisterBotProvider().RegisterDataverseApi().RegisterSqlApi();

    private static IServiceCollection RegisterBlobBotStorage(this IServiceCollection services)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(
            "BlobBotStorage")
        .UsePollyStandard()
        .UseHttpApi()
        .UseBlobBotStorage(
            "BlobBotStorage")
        .ToRegistrar(
            services)
        .RegisterSingleton();

    private static IServiceCollection RegisterBotProvider(this IServiceCollection services)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(
            "BotApi")
        .UsePollyStandard()
        .ConfigureHttpHeader(
            "Ocp-Apim-Subscription-Key", "TelegramBot:ApiKey")
        .UseHttpApi(
            "TelegramBot")
        .UseTelegramBotApi()
        .With<IBotStorage>(
            ServiceProviderServiceExtensions.GetRequiredService<IBlobBotStorage>)
        .UseBotProvider(
            "Bot")
        .ToRegistrar(
            services)
        .RegisterSingleton();

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