using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

partial class Application
{
    private static IBotBuilder UseIncidentCreateFlow(this IBotBuilder botBuilder)
        =>
        UseDataverseApiClient().UseSupportApi()
        .With(UseSupportGptApi())
        .With(ResolveIncidentCreateFlowOption)
        .MapIncidentCreateFlow(botBuilder);

    private static Dependency<ISupportGptApi> UseSupportGptApi()
        =>
        UseHttpMessageHandlerStandard("SupportGptApi")
        .With(ResolveSupportGptApiOption)
        .UseSupportGptApi();

    private static IncidentCreateFlowOption ResolveIncidentCreateFlowOption(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetConfiguration();

        var baseUri = new Uri(configuration.GetDataverseApiClientOption(DataverseSectionName).ServiceUrl);
        var template = configuration.GetValue<string>("IncidentCardRelativeUrlTemplate");

        var uri = new Uri(baseUri, template.OrEmpty()).AbsoluteUri;

        return new(
            incidentCardUrlTemplate: uri.Replace("%7B", "{").Replace("%7D", "}"));
    }

    private static SupportGptApiOption ResolveSupportGptApiOption(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetConfiguration();

        var gptApiSection = configuration.GetRequiredSection("GptApi");
        var incidentCompleteSection = gptApiSection.GetRequiredSection("IncidentComplete");

        return new(
            apiKey: gptApiSection.GetValue<string>("Key").OrEmpty(),
            incidentComplete: new IncidentCompleteOption(
                model: incidentCompleteSection.GetValue<string>("Model").OrEmpty())
            {
                MaxTokens = incidentCompleteSection.GetValue<int?>("MaxTokens"),
                Temperature = incidentCompleteSection.GetValue<decimal?>("Temperature"),
                ChatMessages = new(
                    new(
                        role: "system",
                        contentTemplate: incidentCompleteSection.GetValue<string>("SystemTemplate").OrEmpty()),
                    new(
                        role: "user",
                        contentTemplate: incidentCompleteSection.GetValue<string>("UserTemplate").OrEmpty()))
            });
    }
}