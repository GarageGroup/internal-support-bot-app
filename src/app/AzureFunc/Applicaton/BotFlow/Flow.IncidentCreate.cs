using System;
using System.Collections.Generic;
using System.Text;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private const string GptApiSectionName = "GptApi";

    private const string IncidentCompleteSectionName = "IncidentComplete";

    private static IBotBuilder UseIncidentCreateFlow(this IBotBuilder botBuilder)
        =>
        Dependency.From(
            ResolveIncidentCreateFlowOption)
        .With(
            UseDataverseApi().With(UseSqlApi()).UseCrmCustomerApi())
        .With(
            UseDataverseApi().With(UseSqlApi()).UseCrmContactApi())
        .With(
            UseDataverseApi().UseCrmOwnerApi())
        .With(
            UseDataverseApi().UseCrmIncidentApi())
        .With(
            UseHttpMessageHandlerStandard("SupportGptApi").With(ResolveSupportGptApiOption).UseSupportGptApi())
        .MapIncidentCreateFlow(botBuilder);

    private static IncidentCreateFlowOption ResolveIncidentCreateFlowOption(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetConfiguration();

        var baseUri = new Uri(configuration.GetDataverseApiClientOption(DataverseSectionName).ServiceUrl);
        var template = configuration["IncidentCardRelativeUrlTemplate"];

        var uri = new Uri(baseUri, template.OrEmpty()).AbsoluteUri;

        return new(
            incidentCardUrlTemplate: uri.Replace("%7B", "{").Replace("%7D", "}"),
            dbRequestPeriodInDays: configuration.GetValue<int>("DbRequestPeriodInDays"))
        {
            GptTraceData = configuration.GetGptTraceData()
        };
    }

    private static SupportGptApiOption ResolveSupportGptApiOption(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetConfiguration();

        var gptApiSection = configuration.GetRequiredSection(GptApiSectionName);
        var incidentCompleteSection = gptApiSection.GetRequiredSection(IncidentCompleteSectionName);

        return new(
            apiKey: gptApiSection["Key"].OrEmpty(),
            incidentComplete: new IncidentCompleteOption(
                model: incidentCompleteSection["Model"].OrEmpty())
            {
                MaxTokens = incidentCompleteSection.GetValue<int?>("MaxTokens"),
                Temperature = incidentCompleteSection.GetValue<decimal?>("Temperature"),
                ChatMessages = new(
                    new(
                        role: "system",
                        contentTemplate: incidentCompleteSection["SystemTemplate"].OrEmpty()),
                    new(
                        role: "user",
                        contentTemplate: incidentCompleteSection["UserTemplate"].OrEmpty()))
            });
    }

    private static FlatArray<KeyValuePair<string, string>> GetGptTraceData(this IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection(GptApiSectionName).GetRequiredSection(IncidentCompleteSectionName);
        var traceData = new Dictionary<string, string>();

        foreach (var child in section.GetChildren())
        {
            if (string.IsNullOrEmpty(child.Value) is false)
            {
                traceData[child.Key.FromLowerCase()] = child.Value;
            }
        }

        return traceData.ToFlatArray();
    }

    private static string FromLowerCase(this string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.Empty;
        }

        var firstSymbol = char.ToLowerInvariant(source[0]).ToString();
        if (source.Length is 1)
        {
            return firstSymbol;
        }

        return new StringBuilder(firstSymbol).Append(source[1..]).ToString();
    }
}