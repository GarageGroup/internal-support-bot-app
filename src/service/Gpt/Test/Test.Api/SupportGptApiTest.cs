using GarageGroup.Infra;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

public static partial class SupportGptApiTest
{
    private static readonly SupportGptApiOption SomeOption
        =
        new(
            chatMessages:
            [
                new("some-role", "Some message: {0}")
            ])
        {
            MaxTokens = 150,
            Temperature = 0.7m,
            CaseTypeTemplate = new("someCaseTypeTemplate-role", "SomeCaseTypeTemplate message")
        };

    private static readonly IncidentCompleteIn SomeInput
        =
        new("Some customer message");

    private static readonly HttpSendOut SomeSuccessOutput
        =
        new()
        {
            StatusCode = HttpSuccessCode.OK,
            Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
            {
                Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "Some response \"message\""
                                },
                                FinishReason = "stop"
                            }
                        ]
            }),
        };

    private static Mock<IHttpApi> BuildMockHttpApi(
        in Result<HttpSendOut, HttpSendFailure> titleResult,
        in Result<HttpSendOut, HttpSendFailure> caseTypeResult)
    {
        var queue = new Queue<Result<HttpSendOut, HttpSendFailure>>([titleResult,  caseTypeResult]);

        var mock = new Mock<IHttpApi>();

        _ = mock.Setup(static a => a.SendAsync(It.IsAny<HttpSendIn>(), It.IsAny<CancellationToken>())).ReturnsAsync(queue.Dequeue);

        return mock;
    }
}