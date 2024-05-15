using GarageGroup.Infra;
using Moq;
using System;
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
            Temperature = 0.7m
        };

    private static readonly IncidentCompleteIn SomeInput
        =
        new("Some customer message");

    private static readonly HttpSendOut SomeSuccessOutput
        =
        new()
        {
            StatusCode = HttpSuccessCode.OK
        };

    private static Mock<IHttpApi> BuildMockHttpApi(in Result<HttpSendOut, HttpSendFailure> result)
    {
        var mock = new Mock<IHttpApi>();

        _ = mock.Setup(static a => a.SendAsync(It.IsAny<HttpSendIn>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        return mock;
    }
}