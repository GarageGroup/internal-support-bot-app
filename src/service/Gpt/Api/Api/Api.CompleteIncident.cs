using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

partial class SupportGptApi
{
    public ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> CompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>>(cancellationToken);
        }

        return InnerCompleteIncidentAsync(input, cancellationToken);
    }

    private async ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> InnerCompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Message))
        {
            return default(IncidentCompleteOut);
        }

        var sourceMessage = input.Message.Trim();
        var jsonIn = new ChatGptJsonIn
        {
            Model = option.Model,
            MaxTokens = option.IncidentComplete.MaxTokens,
            Temperature = option.IncidentComplete.Temperature,
            Top = 1,
            Messages = option.IncidentComplete.ChatMessages.Map(CreateChatMessageJson)
        };

        using var httpClient = CreateHttpClient();
        using var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = JsonContent.Create(jsonIn, new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
        };

        using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        var httpResponseText = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (httpResponse.StatusCode is HttpStatusCode.TooManyRequests)
        {
            return Failure.Create(
                failureCode: IncidentCompleteFailureCode.TooManyRequests,
                failureMessage: ReadErrorMessage(httpResponseText));
        }

        if (httpResponse.IsSuccessStatusCode is false)
        {
            return Failure.Create(
                failureCode: IncidentCompleteFailureCode.Unknown,
                failureMessage: $"An unexpected http status code: {httpResponse.StatusCode}. Body: '{httpResponseText}'");
        }

        var jsonOut = JsonSerializer.Deserialize<ChatGptJsonOut>(httpResponseText);

        if (jsonOut.Choices.IsEmpty)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"GPT result choices are absent. Body: '{httpResponseText}'");
        }

        var choice = jsonOut.Choices[0];
        if (string.Equals(choice.FinishReason, "stop", StringComparison.InvariantCultureIgnoreCase) is false)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"An unexpected GPT finish reason: '{choice.FinishReason}'. Body: '{httpResponseText}'");
        }

        return new IncidentCompleteOut
        {
            Title = TrimTitle(choice.Message?.Content)
        };

        ChatMessageJson CreateChatMessageJson(ChatMessageOption messageOption)
            =>
            new()
            {
                Role = messageOption.Role,
                Content = string.Format(messageOption.ContentTemplate, sourceMessage)
            };
    }

    private static string? ReadErrorMessage(string? httpResponseText)
    {
        if (string.IsNullOrEmpty(httpResponseText))
        {
            return null;
        }

        var message = JsonSerializer.Deserialize<ChatFailureJson>(httpResponseText)?.Error?.Message;
        if (string.IsNullOrWhiteSpace(message))
        {
            return httpResponseText;
        }

        return message;
    }
}