using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using PrimeFuncPack.UnitTest;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTest
{
    [Theory]
    [InlineData(null)]
    [InlineData(TestData.EmptyString)]
    [InlineData(TestData.MixedWhiteSpacesString)]
    public static async Task CompleteIncidentAsync_InputMessageIsNullOrWhiteSpace_ExpectDefaultIncidentCompletion(
        string? inputMessage)
    {
        var mockHttpApi = BuildMockHttpApi(SomeSuccessOutput);

        var api = new SupportGptApi(mockHttpApi.Object, SomeOption);

        var input = new IncidentCompleteIn(inputMessage!);
        var cancellationToken = new CancellationToken(canceled: false);

        var actual = await api.CompleteIncidentAsync(input, cancellationToken);
        var expected = default(IncidentCompleteOut);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.InputTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_InputMessageIsNotWhiteSpace_ExpectHttpApiSendAsyncOnce(
        SupportGptApiOption option, IncidentCompleteIn input, HttpSendIn htppInput)
    {
        var mockHttpApi = BuildMockHttpApi(SomeSuccessOutput);

        var api = new SupportGptApi(mockHttpApi.Object, option);

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.CompleteIncidentAsync(input, cancellationToken);

        mockHttpApi.Verify(a => a.SendAsync(htppInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.OutputFailureTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpApiIsNotSuccess_ExpectFailure(
        HttpSendFailure httpSendFailure, Failure<IncidentCompleteFailureCode> failureExpected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);

        var api = new SupportGptApi(mockHttpApi.Object, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.StrictEqual(failureExpected, actual);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.OutputSuccessTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpStatusCodeIsSuccessAndJsonIsExpected_ExpectSuccess(
        HttpSendOut output, Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>> expected)
    {
        var mockHttpApi = BuildMockHttpApi(output);

        var api = new SupportGptApi(mockHttpApi.Object, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.StrictEqual(expected, actual);
    }
}