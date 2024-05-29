using GarageGroup.Infra;
using GarageGroup.Internal.Support.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

partial class SupportGptApi
{
    public ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> CompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Message) && input.ImageUrls.IsEmpty)
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
            SendHttpApiAsync)
        .Forward(
            MapTitleSuccessOrFailure)
        .MapSuccess(
            @out => (input, @out))
        .ForwardValue(
            GetCaseTypeAsync);

    private async ValueTask<Result<HttpSendOut, Failure<IncidentCompleteFailureCode>>> SendHttpApiAsync(
        HttpSendIn request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await gptHttp.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return result.MapFailure(MapFailure);
        }
        catch (TaskCanceledException ex)
        {
            return ex.ToFailure(IncidentCompleteFailureCode.ExceededTimeout, "Operation is cancelled");
        }

        static Failure<IncidentCompleteFailureCode> MapFailure(HttpSendFailure failure)
            =>
            failure.ToStandardFailure().MapFailureCode(ToIncidentCompleteFailureCode);
    }

    private ValueTask<Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> GetCaseTypeAsync(
        (IncidentCompleteIn Input, IncidentCompleteOut Output) request, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            request.Input, cancellationToken)
        .Pipe(
            MapCaseTypeInput)
        .Pipe(
            static @in => new HttpSendIn(
                method: HttpVerb.Post,
                requestUri: string.Empty)
            {
                Body = HttpBody.SerializeAsJson(@in)
            })
        .PipeValue(
            SendHttpApiAsync)
        .Forward(
            @out => MapCaseTypeSuccessOrFailure(@out, request.Output));

    private ChatGptJsonIn MapCaseTypeInput(IncidentCompleteIn input)
    {
        return new()
        {
            MaxTokens = option.MaxTokens,
            Temperature = option.Temperature,
            Top = 1,
            Messages = option.ChatMessages.Map(CreateChatMessageJson).Concat(
                new ChatMessageJsonIn
                {
                    Role = option.CaseTypeTemplate?.Role,
                    Content =
                    [
                        new(
                            text: option.CaseTypeTemplate?.ContentTemplate)
                    ]
                })
        };

        ChatMessageJsonIn CreateChatMessageJson(ChatMessageOption messageOption)
            =>
            new()
            {
                Role = messageOption.Role,
                Content = CreateChatContentJsonIn(input, messageOption)
            };
    }

    private ChatGptJsonIn MapTitleInput(IncidentCompleteIn input)
    {
        return new()
        {
            MaxTokens = option.MaxTokens,
            Temperature = option.Temperature,
            Top = 1,
            Messages = option.ChatMessages.Map(CreateChatMessageJson)
        };

        ChatMessageJsonIn CreateChatMessageJson(ChatMessageOption messageOption)
            =>
            new()
            {
                Role = messageOption.Role,
                Content = CreateChatContentJsonIn(input, messageOption)
            };
    }

    private static FlatArray<ChatContentJsonIn> CreateChatContentJsonIn(IncidentCompleteIn input, ChatMessageOption messageOption)
    {
        if (messageOption.Role.Equals("system", StringComparison.InvariantCultureIgnoreCase))
        {
            return [new(text: messageOption.ContentTemplate)];
        }

        if (string.IsNullOrWhiteSpace(input.Message))
        {
            return input.ImageUrls.Map(CreateChatContentJsonIn);
        }

        if (input.ImageUrls.IsEmpty)
        {
            return [new(text: string.Format(messageOption.ContentTemplate, input.Message.Trim()))];
        }

        return input.ImageUrls.Map(CreateChatContentJsonIn).Concat(
            new ChatContentJsonIn(text: string.Format(messageOption.ContentTemplate, input.Message.Trim())));

        static ChatContentJsonIn CreateChatContentJsonIn(string imageUrl)
            =>
            new(image: new(imageUrl));
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
}