using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage response;

    private readonly Func<HttpRequestMessage, Task>? callbackAsync;

    private int callsCount = 0;

    private Exception? innerException = null;

    internal MockHttpMessageHandler(HttpResponseMessage response, Func<HttpRequestMessage, Task>? callbackAsync = null)
    {
        this.response = response;
        this.callbackAsync = callbackAsync;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            callsCount++;

            if (callbackAsync is not null)
            {
                await callbackAsync.Invoke(request);
            }

            return response;
        }
        catch (Exception ex)
        {
            innerException = ex;
            throw;
        }
    }

    internal void Verify(int expectedTimes)
    {
        if (expectedTimes != callsCount)
        {
            throw new InvalidOperationException(
                $"HttpMessageHandler was expected to be called {expectedTimes} times but it was {callsCount}");
        }

        if (innerException is not null)
        {
            throw innerException;
        }
    }
}