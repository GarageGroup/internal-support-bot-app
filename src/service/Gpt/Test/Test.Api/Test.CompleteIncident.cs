using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PrimeFuncPack.UnitTest;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTest
{
    [Fact]
    public static async Task CompleteIncidentAsync_InputIsNull_ExpectArgumentNullException()
    {
        using var response = CreateSuccessResponse(SomeResponseMessage);
        using var messageHandler = new MockHttpMessageHandler(response);

        var api = CreateSupportGptApi(messageHandler, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);

        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.CompleteIncidentAsync(null!, cancellationToken);
    }

    [Fact]
    public static void CompleteIncidentAsync_CancellationTokenIsCanceled_ExpectCanceledTask()
    {
        using var response = CreateSuccessResponse(SomeResponseMessage);
        using var messageHandler = new MockHttpMessageHandler(response);

        var api = CreateSupportGptApi(messageHandler, SomeOption);

        var cancellationToken = new CancellationToken(canceled: true);
        var actual = api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.True(actual.IsCanceled);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(TestData.EmptyString)]
    [InlineData(TestData.MixedWhiteSpacesString)]
    public static async Task CompleteIncidentAsync_InputMessageIsNullOrWhiteSpace_ExpectDefaultIncidentCompletion(
        string inputMessage)
    {
        using var response = CreateSuccessResponse(SomeResponseMessage);
        using var messageHandler = new MockHttpMessageHandler(response);

        var api = CreateSupportGptApi(messageHandler, SomeOption);

        var input = new IncidentCompleteIn(inputMessage);
        var cancellationToken = new CancellationToken(canceled: false);

        var actual = await api.CompleteIncidentAsync(input, cancellationToken);
        var expected = default(IncidentCompleteOut);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.InputTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_InputMessageIsNotWhiteSpace_ExpectMessageHandlerCalledOnce(
        SupportGptApiOption option, IncidentCompleteIn input, string expectedContent, string expectedAuthorization)
    {
        using var response = CreateSuccessResponse(SomeResponseMessage);
        using var messageHandler = new MockHttpMessageHandler(response, OnRequestSentAsync);

        var api = CreateSupportGptApi(messageHandler, option);

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.CompleteIncidentAsync(input, cancellationToken);

        messageHandler.Verify(1);

        async Task OnRequestSentAsync(HttpRequestMessage actual)
        {
            Assert.Equal(HttpMethod.Post, actual.Method);

            var actualRequestUrl = actual.RequestUri?.ToString();
            Assert.Equal("https://api.openai.com/v1/chat/completions", actualRequestUrl, ignoreCase: true);

            Assert.NotNull(actual.Content);

            var actualContent = await actual.Content.ReadAsStringAsync();
            Assert.Equal(expectedContent, actualContent);

            var actualAuthorization = actual.Headers.Authorization?.ToString();
            Assert.Equal(expectedAuthorization, actualAuthorization);
        }
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.OutputFailureTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpStatusCodeIsNotSuccessNorJsonIsNotExpected_ExpectFailure(
        HttpStatusCode statusCode, string? responseContent, Failure<IncidentCompleteFailureCode> expected)
    {
        using var response = new HttpResponseMessage(statusCode)
        {
            Content = responseContent is null ? null : new StringContent(responseContent)
        };

        using var messageHandler = new MockHttpMessageHandler(response);
        var api = CreateSupportGptApi(messageHandler, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.OutputSuccessTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpStatusCodeIsSuccessAndJsonIsExpected_ExpectSuccess(
        HttpStatusCode statusCode, string responseContent, IncidentCompleteOut expected)
    {
        using var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(responseContent)
        };

        using var messageHandler = new MockHttpMessageHandler(response);
        var api = CreateSupportGptApi(messageHandler, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.StrictEqual(expected, actual);
    }
}