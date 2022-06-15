using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

partial class GSupportBotBuilder
{
    internal static IBotBuilder UseGSupportIncidentCreate(this IBotBuilder botBuilder)
        =>
        botBuilder.UseIncidentCreate(GetIncidentCreateBotOption, GetCustomerSetSearchApi, GetIncidentCreateApi, GetContactSetSearchApi);

    private static ICustomerSetSearchFunc GetCustomerSetSearchApi(IBotContext botContext)
        =>
        CreateStandardHttpHandlerDependency("CustomerSetSearchApi")
        .CreateDataverseApiClient()
        .UseCustomerSetSearchApi()
        .Resolve(botContext.ServiceProvider);

    private static IIncidentCreateFunc GetIncidentCreateApi(IBotContext botContext)
        =>
        CreateStandardHttpHandlerDependency("IncidentCreateApi")
        .CreateDataverseApiClient()
        .UseIncidentCreateApi()
        .Resolve(botContext.ServiceProvider);

    private static IContactSetSearchFunc GetContactSetSearchApi(IBotContext botContext)
        =>
        CreateStandardHttpHandlerDependency("ContactSetSearchApi")
        .CreateDataverseApiClient()
        .UseContactSetSearchApi()
        .Resolve(botContext.ServiceProvider);

    private static IncidentCreateBotOption GetIncidentCreateBotOption(IBotContext botContext)
        =>
        botContext.ServiceProvider.GetRequiredService<IConfiguration>().GetIncidentCreateBotOption();

    private static IncidentCreateBotOption GetIncidentCreateBotOption(this IConfiguration configuration)
    {
        var baseUri = new Uri(configuration.GetDataverseApiClientOption().ServiceUrl);
        var template = configuration.GetValue<string>("IncidentCardRelativeUrlTemplate");

        var uri = new Uri(baseUri, template.OrEmpty()).AbsoluteUri;
        return new(
            incidentCardUrlTemplate: uri.ReplaceInvariant("%7B", "{").ReplaceInvariant("%7D", "}"),
            caseOriginCode: configuration.GetValue<int?>("CaseOriginCode"));
    }
}