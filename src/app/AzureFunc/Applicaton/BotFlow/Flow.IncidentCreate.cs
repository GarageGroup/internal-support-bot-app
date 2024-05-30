using System;
using System.Collections.Generic;
using System.Text;
using GarageGroup.Infra;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static BotCommandBuilder WithIncidentCreateCommand(this BotCommandBuilder builder)
        =>
        builder.With(
            UseIncidentCreateCommand());

    private static Dependency<IChatCommand<IncidentCreateCommandIn, Unit>> UseIncidentCreateCommand()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("CrmIncidentFileApi")
        .UseHttpApi()
        .With(UseDataverseApi())
        .UseCrmIncidentApi()
        .With(
            UseDataverseApi().With(UseSqlApi()).UseCrmOwnerApi())
        .With(
            UseSupportGptApi())
        .With(
            ServiceProviderServiceExtensions.GetService<TelemetryClient>,
            ResolveIncidentCreateFlowOption)
        .UseIncidentCreateCommand();

    private static Dependency<ISupportGptApi> UseSupportGptApi()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("SupportGptApi")
        .UsePollyStandard()
        .ConfigureHttpHeader("api-key", "GptApi:Azure:Key")
        .UseHttpApi("GptApi:Azure")
        .With(ResolveSupportGptApiOption)
        .UseSupportGptApi();

    private static IncidentCreateFlowOption ResolveIncidentCreateFlowOption(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetConfiguration();

        var baseUri = new Uri(configuration.GetDataverseApiClientOption(ApplicationHost.DataverseSectionName).ServiceUrl);
        var template = configuration["IncidentCardRelativeUrlTemplate"];

        var uri = new Uri(baseUri, template.OrEmpty()).AbsoluteUri;

        return new(
            incidentCardUrlTemplate: uri.Replace("%7B", "{").Replace("%7D", "}"))
        {
            GptTraceData = configuration.GetGptTraceData()
        };
    }

    private static SupportGptApiOption ResolveSupportGptApiOption(IServiceProvider serviceProvider)
    {
        var section = serviceProvider.GetConfiguration().GetRequiredSection("GptApi:IncidentComplete");

        return new(
            [
                new(
                    role: "system",
                    contentTemplate: section["SystemTemplate"].OrEmpty()),
                new(
                    role: "user",
                    contentTemplate: section["UserIncidentTitleTemplate"].OrEmpty())
            ])
        {
            MaxTokens = section.GetValue<int?>("MaxTokens"),
            Temperature = section.GetValue<decimal?>("Temperature"),
            IsImageProcessing = section.GetValue<bool>("IsImageProcessing"),
            CaseTypeTemplate = new(
                role: "user",
                contentTemplate: section["UserIncidentCaseTypeTemplate"].OrEmpty())
        };
    }

    private static FlatArray<KeyValuePair<string, string>> GetGptTraceData(this IConfiguration configuration)
    {
        var gptApiSection = configuration.GetRequiredSection("GptApi");
        var traceData = new Dictionary<string, string>();

        traceData.AppendSectionValues(gptApiSection);
        traceData.AppendSectionValues(gptApiSection.GetSection("IncidentComplete"));

        return traceData.ToFlatArray();
    }

    private static void AppendSectionValues(this Dictionary<string, string> traceData, IConfigurationSection section)
    {
        if (section.Exists() is false)
        {
            return;
        }

        foreach (var child in section.GetChildren())
        {
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