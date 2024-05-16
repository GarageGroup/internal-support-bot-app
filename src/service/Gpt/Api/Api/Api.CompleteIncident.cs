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
            MapTitleInput)
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
            MapTitleSuccessOrFailure,
            static failure => failure.ToStandardFailure().MapFailureCode(ToIncidentCompleteFailureCode))
        .MapSuccess(
            @out => (input, @out))
        .ForwardValue(
            GetCaseTypeAsync);

    private ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> GetCaseTypeAsync(
        (IncidentCompleteIn Input, IncidentCompleteOut Output) input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            @in => MapCaseTypeInput(@in.Input))
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
            @out => MapCaseTypeSuccessOrFailure(@out, input.Output),
            static failure => failure.ToStandardFailure().MapFailureCode(ToIncidentCompleteFailureCode));

    private ChatGptJsonIn MapCaseTypeInput(IncidentCompleteIn input)
    {
        var sourceMessage = input.Message.Trim();

        return new()
        {
            MaxTokens = option.MaxTokens,
            Temperature = option.Temperature,
            Top = 1,
            Messages = option.ChatMessages.Map(CreateChatMessageJson).Concat(
                [
                    new()
                    {
                        Role = option.CaseTypeTemplate?.Role,
                        Content = option.CaseTypeTemplate?.ContentTemplate
                    }
                ])
        };

        ChatMessageJson CreateChatMessageJson(ChatMessageOption messageOption)
            =>
            new()
            {
                Role = messageOption.Role,
                Content = string.Format(messageOption.ContentTemplate, sourceMessage)
            };
    }

    private ChatGptJsonIn MapTitleInput(IncidentCompleteIn input)
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

    private static Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>> MapTitleSuccessOrFailure(HttpSendOut httpResponse)
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

    private static Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>> MapCaseTypeSuccessOrFailure(
        HttpSendOut httpResponse, IncidentCompleteOut output)
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

        if (int.TryParse(choice.Message?.Content, out var caseType) is false || caseType < 0 || caseType > 2)
        {
            return Failure.Create(
                IncidentCompleteFailureCode.Unknown,
                $"Incorrect response from GPT. Body: '{httpResponse.Body}'");
        }

        return output with
        {
            CaseTypeCode = (IncidentCaseTypeCode)caseType
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