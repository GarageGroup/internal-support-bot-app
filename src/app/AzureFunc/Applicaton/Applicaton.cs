using System;
using System.Net;
using System.Net.Http;
using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

[HealthCheckFunc("HealthCheck", AuthLevel = AuthorizationLevel.Function)]
internal static partial class Application
{
    private const string DataverseSectionName = "Dataverse";

    private const string GptApiSectionName = "GptApi";

    private const string GptApiAzureSectionName = "Azure";

    private const string IncidentCompleteSectionName = "IncidentComplete";

    private const string BotEntityName = "BotRequest";

    private static Dependency<HttpMessageHandler> UseHttpMessageHandlerStandard(string loggerCategoryName)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(loggerCategoryName)
        .UsePollyStandard(HttpStatusCode.TooManyRequests);

    private static Dependency<IDataverseApiClient> UseDataverseApi()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<IDataverseApiClient>);

    private static Dependency<ISqlApi> UseSqlApi()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<ISqlApi>);

    private static IConfiguration GetConfiguration(this IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredService<IConfiguration>();
}