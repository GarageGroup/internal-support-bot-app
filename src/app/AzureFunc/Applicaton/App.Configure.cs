using System;
using System.Text;
using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
        UseHttpMessageHandlerStandard("DataverseApi")
        .UseDataverseApiClient(DataverseSectionName)
        .ToRegistrar(services)
        .RegisterScoped();

    private static IServiceCollection RegisterSqlApi(this IServiceCollection services)
        =>
        MicrosoftDbProvider.Configure(ResolveDataverseDbProviderOption)
        .UseSqlApi()
        .ToRegistrar(services)
        .RegisterScoped();

    private static MicrosoftDbProviderOption ResolveDataverseDbProviderOption(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetConfiguration();
        var option = configuration.GetDataverseApiClientAuthOption(DataverseSectionName);

        var connectionStringBuilder = new StringBuilder()
            .Append("Server=").Append(new Uri(option.ServiceUrl).Host).Append(",5558;")
            .Append("Initial Catalog=").Append(configuration[$"{DataverseSectionName}:EnvironmentId"]).Append(';')
            .Append("Authentication=ActiveDirectoryServicePrincipal;")
            .Append("User ID=").Append(option.AuthClientId).Append(';')
            .Append("Password=").Append(option.AuthClientSecret).Append(';');

        return new(
            connectionString: connectionStringBuilder.ToString(),
            retryOption: configuration.GetSection($"{DataverseSectionName}:DatabaseRetryPolicy").Get<SqlRetryLogicOption>());
    }
}