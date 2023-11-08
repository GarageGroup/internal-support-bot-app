using System;
using System.Net.Http;

namespace GarageGroup.Internal.Support;

internal sealed partial class SupportGptApi : ISupportGptApi
{
    private const string OpenAiBaseAddressUrl = "https://api.openai.com/";

    private const string OpenAiCompletionsUrl = "/v1/chat/completions";

    private readonly HttpMessageHandler httpMessageHandler;

    private readonly SupportGptApiOption option;

    internal SupportGptApi(HttpMessageHandler httpMessageHandler, SupportGptApiOption option)
    {
        this.httpMessageHandler = httpMessageHandler;
        this.option = option;
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient(httpMessageHandler, false)
        {
            BaseAddress = new(OpenAiBaseAddressUrl)
        };

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {option.ApiKey}");
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