using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTest
{
    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.InputInvalidTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_InputMessageIsNullOrWhiteSpace_ExpectDefaultIncidentCompletion(
        SupportGptApiOption option, IncidentCompleteIn input)
    {
        var mockHttpApi = BuildMockHttpApi(SomeSuccessOutput, SomeSuccessOutput);

        var api = new SupportGptApi(mockHttpApi.Object, option);

        var cancellationToken = new CancellationToken(canceled: false);

        var actual = await api.CompleteIncidentAsync(input, cancellationToken);
        var expected = default(IncidentCompleteOut);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.InputTitleTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_InputMessageIsNotWhiteSpace_ExpectTitleHttpApiSendAsyncOnce(
        SupportGptApiOption option, IncidentCompleteIn input, HttpSendIn httpInput)
    {
        var mockHttpApi = BuildMockHttpApi(SomeSuccessOutput, SomeSuccessOutput);

        var api = new SupportGptApi(mockHttpApi.Object, option);

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.CompleteIncidentAsync(input, cancellationToken);

        mockHttpApi.Verify(a => a.SendAsync(httpInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.OutputFailureTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpApiTitleIsNotSuccess_ExpectFailure(
        HttpSendFailure httpSendFailure, Failure<IncidentCompleteFailureCode> failureExpected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure, SomeSuccessOutput);

        var api = new SupportGptApi(mockHttpApi.Object, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.StrictEqual(failureExpected, actual);
    }

    [Fact]
    public static async Task CompleteIncidentAsync_HttpApiTitleIsTaskCanceledException_ExpectFailure()
    {
        var exception = new TaskCanceledException("Some exception message");
        var mockHttpApi = BuildMockHttpApiWithException(exception);

        var api = new SupportGptApi(mockHttpApi.Object, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        var expected = exception.ToFailure(IncidentCompleteFailureCode.ExceededTimeout, "Operation is cancelled");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.InputCaseTypeTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpApiTitleIsSuccess_ExpectCaseTypeHttpApiSendAsyncOnce(
        SupportGptApiOption option, IncidentCompleteIn input, HttpSendIn httpInput)
    {
        var mockHttpApi = BuildMockHttpApi(SomeSuccessOutput, SomeSuccessOutput);

        var api = new SupportGptApi(mockHttpApi.Object, option);

        var cancellationToken = new CancellationToken(canceled: false);
        _ = await api.CompleteIncidentAsync(input, cancellationToken);

        mockHttpApi.Verify(a => a.SendAsync(httpInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.OutputFailureTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpApiCaseTypeIsNotSuccess_ExpectFailure(
        HttpSendFailure httpSendFailure, Failure<IncidentCompleteFailureCode> failureExpected)
    {
        var mockHttpApi = BuildMockHttpApi(SomeSuccessOutput, httpSendFailure);

        var api = new SupportGptApi(mockHttpApi.Object, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.StrictEqual(failureExpected, actual);
    }

    [Theory]
    [MemberData(nameof(SupportGptApiTestSource.OutputSuccessTestData), MemberType = typeof(SupportGptApiTestSource))]
    public static async Task CompleteIncidentAsync_HttpApiIsSuccess_ExpectSuccessOrFailure(
        HttpSendOut titleOutput, HttpSendOut caseTypeOutput, Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>> expected)
    {
        var mockHttpApi = BuildMockHttpApi(titleOutput, caseTypeOutput);

        var api = new SupportGptApi(mockHttpApi.Object, SomeOption);

        var cancellationToken = new CancellationToken(canceled: false);
        var actual = await api.CompleteIncidentAsync(SomeInput, cancellationToken);

        Assert.StrictEqual(expected, actual);
    }
}