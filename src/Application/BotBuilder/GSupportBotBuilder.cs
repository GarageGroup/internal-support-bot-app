using System;
using System.Net.Http;
using GGroupp.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

internal static partial class GSupportBotBuilder
{
    private static Dependency<HttpMessageHandler> CreateStandardHttpHandlerDependency(string loggerCategoryName)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(
            sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger(loggerCategoryName.OrEmpty()));

    private static Dependency<IDataverseApiClient> CreateDataverseApiClient(this Dependency<HttpMessageHandler> dependency)
        =>
        dependency.UseDataverseApiClient(
            sp => sp.GetRequiredService<IConfiguration>().GetDataverseApiClientOption());

    private static DataverseApiClientOption GetDataverseApiClientOption(this IConfiguration configuration)
        =>
        new(
            serviceUrl: configuration.GetValue<string>("DataverseApiServiceUrl"),
            authTenantId: configuration.GetValue<string>("DataverseApiAuthTenantId"),
            authClientId: configuration.GetValue<string>("DataverseApiAuthClientId"),
            authClientSecret: configuration.GetValue<string>("DataverseApiAuthClientSecret"));

    private static string ReplaceInvariant(this string source, string oldValue, string newValue)
        =>
        source.Replace(oldValue, newValue, StringComparison.InvariantCultureIgnoreCase);
}