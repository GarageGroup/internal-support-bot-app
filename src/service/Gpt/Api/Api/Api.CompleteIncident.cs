using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

partial class SupportGptApi
{
    public ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> CompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);

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
            Model = option.IncidentComplete.Model,
            MaxTokens = option.IncidentComplete.MaxTokens,
            Temperature = option.IncidentComplete.Temperature,
            Top = 1,
            Messages = option.IncidentComplete.ChatMessages.Map(CreateChatMessageJson)
        };

        var json = JsonSerializer.Serialize(jsonIn);
        using var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var httpClient = CreateHttpClient();

        using var httpResponse = await httpClient.PostAsync(OpenAiCompletionsUrl, content, cancellationToken).ConfigureAwait(false);
        var httpResponseText = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (httpResponse.StatusCode is HttpStatusCode.TooManyRequests)
        {
            var errorMessage = ReadErrorMessage(httpResponseText);
            return Failure.Create(IncidentCompleteFailureCode.TooManyRequests, errorMessage);
        }

        if (httpResponse.IsSuccessStatusCode is false)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"An unexpected http status code: {httpResponse.StatusCode}. Body: {httpResponseText}");
        }

        var jsonOut = JsonSerializer.Deserialize<ChatGptJsonOut>(httpResponseText);

        if (jsonOut.Choices.IsEmpty)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"GPT result choices are absent. Body: {httpResponseText}");
        }

        var choice = jsonOut.Choices[0];
        if (string.Equals(choice.FinishReason, "stop", StringComparison.InvariantCultureIgnoreCase) is false)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"An unexpected GPT finish reason: {choice.FinishReason}. Body: {httpResponseText}");
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