using GarageGroup.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

partial class SupportGptApi
{
    public ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> CompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Message))
        {
            return new(default(IncidentCompleteOut));
        }

        return InnerCompleteIncidentAsync(input, cancellationToken);
    }

    private ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> InnerCompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            MapInput)
        .Pipe(
            @in => new HttpSendIn(
                method: HttpVerb.Post,
                requestUri: string.Empty)
            {
                Body = HttpBody.SerializeAsJson(@in)
            })
        .PipeValue(
            gptHttp.SendAsync)
        .Forward(
            MapSuccessOrFailure,
            static failure => failure.ToStandardFailure().MapFailureCode(ToIncidentCompleteFailureCode));

    private ChatGptJsonIn MapInput(IncidentCompleteIn input)
    {
        var sourceMessage = input.Message.Trim();

        return new()
        {
            MaxTokens = option.MaxTokens,
            Temperature = option.Temperature,
            Top = 1,
            Messages = option.ChatMessages.Map(CreateChatMessageJson)
        };

        ChatMessageJson CreateChatMessageJson(ChatMessageOption messageOption)
            =>
            new()
            {
                Role = messageOption.Role,
                Content = string.Format(messageOption.ContentTemplate, sourceMessage)
            };
    }

    private static Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>> MapSuccessOrFailure(HttpSendOut httpResponse)
    {
        var jsonOut = httpResponse.Body.DeserializeFromJson<ChatGptJsonOut>();
        if (jsonOut.Choices.IsEmpty)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"GPT result choices are absent. Body: '{httpResponse.Body}'");
        }

        var choice = jsonOut.Choices[0];
        if (string.Equals(choice.FinishReason, "stop", StringComparison.InvariantCultureIgnoreCase) is false)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"An unexpected GPT finish reason: '{choice.FinishReason}'. Body: '{httpResponse.Body}'");
        }

        return new IncidentCompleteOut
        {
            Title = TrimTitle(choice.Message?.Content)
        };
    }

    private static IncidentCompleteFailureCode ToIncidentCompleteFailureCode(HttpFailureCode httpFailureCode)
        =>
        httpFailureCode switch
        {
            HttpFailureCode.TooManyRequests => IncidentCompleteFailureCode.TooManyRequests,
            _ => IncidentCompleteFailureCode.Unknown
        };
}