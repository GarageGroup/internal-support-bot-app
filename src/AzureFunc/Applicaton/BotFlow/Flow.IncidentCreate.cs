using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GGroupp.Internal.Support;

partial class Application
{
    private static IBotBuilder UseIncidentCreateFlow(this IBotBuilder botBuilder)
        =>
        UseDataverseApiClient()
        .UseSupportApi()
        .With(ResolveIncidentCreateFlowOption)
        .MapIncidentCreateFlow(botBuilder);

    private static IncidentCreateFlowOption ResolveIncidentCreateFlowOption(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredService<IConfiguration>().GetIncidentCreateBotOption();

    private static IncidentCreateFlowOption GetIncidentCreateBotOption(this IConfiguration configuration)
    {
        var baseUri = new Uri(configuration.GetDataverseApiClientOption(DataverseSectionName).ServiceUrl);
        var template = configuration.GetValue<string>("IncidentCardRelativeUrlTemplate");

        var uri = new Uri(baseUri, template.OrEmpty()).AbsoluteUri;

        return new(
            incidentCardUrlTemplate: uri.Replace("%7B", "{").Replace("%7D", "}"));
    }
}