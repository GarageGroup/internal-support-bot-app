using System;
using System.Net.Http;

namespace GarageGroup.Internal.Support;

internal sealed partial class SupportGptApi(HttpMessageHandler httpMessageHandler, SupportGptApiOption option) : ISupportGptApi
{
    private const string OpenAiUrl = "https://api.openai.com/v1/chat/completions";

    private const string AzureAiUrlTemplate
        =
        "https://{0}.openai.azure.com/openai/deployments/{1}/chat/completions?api-version={2}";

    private const string AzureAiApiKeyHeaderName = "api-key";

    private HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient(httpMessageHandler, false);
        var azure = option.AzureGpt;

        if (azure is null)
        {
            httpClient.BaseAddress = new(OpenAiUrl);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {option.ApiKey}");
        }
        else
        {
            httpClient.BaseAddress = new(string.Format(AzureAiUrlTemplate, azure.ResourceName, azure.DeploymentId, azure.ApiVersion));
            httpClient.DefaultRequestHeaders.Add(AzureAiApiKeyHeaderName, option.ApiKey);
        }

        return httpClient;
    }

    private static string? TrimTitle(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        var title = source.Trim('.').Trim();
        if (title.StartsWith('"') && title.EndsWith('"'))
        {
            title = title[1..^1];
        }

        return title.Trim('.').Trim().OrNullIfEmpty();
    }
}