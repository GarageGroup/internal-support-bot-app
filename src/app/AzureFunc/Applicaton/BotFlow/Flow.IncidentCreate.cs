using System;
using System.Collections.Generic;
using System.Text;
using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static IBotBuilder UseIncidentCreateFlow(this IBotBuilder botBuilder)
        =>
        Dependency.From(
            ResolveIncidentCreateFlowOption)
        .With(
            UseDataverseApi().With(UseSqlApi()).UseCrmCustomerApi())
        .With(
            UseDataverseApi().With(UseSqlApi()).UseCrmContactApi())
        .With(
            UseDataverseApi().With(UseSqlApi()).UseCrmOwnerApi())
        .With(
            UseDataverseApi().UseCrmIncidentApi())
        .With(
            UseSupportGptApi())
        .MapIncidentCreateFlow(botBuilder);

    private static Dependency<ISupportGptApi> UseSupportGptApi()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("SupportGptApi")
        .UsePollyStandard()
        .With(ResolveSupportGptApiOption)
        .UseSupportGptApi();

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

        var incidentComplete = new IncidentCompleteOption(
            chatMessages:
            [
                new(
                    role: "system",
                    contentTemplate: incidentCompleteSection["SystemTemplate"].OrEmpty()),
                new(
                    role: "user",
                    contentTemplate: incidentCompleteSection["UserTemplate"].OrEmpty())
            ])
        {
            MaxTokens = incidentCompleteSection.GetValue<int?>("MaxTokens"),
            Temperature = incidentCompleteSection.GetValue<decimal?>("Temperature")
        };

        if (configuration.IsAzureGpt() is false)
        {
            return new(
                apiKey: gptApiSection["Key"].OrEmpty(),
                model: gptApiSection["Model"].OrEmpty(),
                incidentComplete: incidentComplete);
        }

        var azureSection = gptApiSection.GetRequiredSection(GptApiAzureSectionName);

        return new(
            apiKey: azureSection["Key"].OrEmpty(),
            azureGpt: new(
                resourceName: azureSection["ResourceName"].OrEmpty(),
                deploymentId: azureSection["DeploymentId"].OrEmpty(),
                apiVersion: azureSection["ApiVersion"].OrEmpty()),
            incidentComplete: incidentComplete);
    }

    private static FlatArray<KeyValuePair<string, string>> GetGptTraceData(this IConfiguration configuration)
    {
        var gptApiSection = configuration.GetRequiredSection(GptApiSectionName);
        var traceData = new Dictionary<string, string>();

        traceData.AppendSectionValues(gptApiSection);

        if (configuration.IsAzureGpt())
        {
            traceData.AppendSectionValues(gptApiSection.GetSection(GptApiAzureSectionName));
        }

        traceData.AppendSectionValues(gptApiSection.GetSection(IncidentCompleteSectionName));
        return traceData.ToFlatArray();
    }

    private static bool IsAzureGpt(this IConfiguration configuration)
        =>
        string.IsNullOrEmpty(configuration[$"{GptApiSectionName}:Model"]);

    private static void AppendSectionValues(this Dictionary<string, string> traceData, IConfigurationSection section)
    {
        if (section.Exists() is false)
        {
            return;
        }

        foreach (var child in section.GetChildren())
        {
            if (string.Equals("key", child.Key, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (string.IsNullOrEmpty(child.Value) is false)
            {
                traceData[child.Key.FromLowerCase()] = child.Value;
            }
        }
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