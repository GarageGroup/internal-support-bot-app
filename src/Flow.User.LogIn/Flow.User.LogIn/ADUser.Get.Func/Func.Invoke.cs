using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.FormattableString;

namespace GGroupp.Internal.Support.Bot;

partial class ADUserGetFunc
{
    public ValueTask<Result<ADUserGetOut, Failure<Unit>>> InvokeAsync(
        ADUserGetIn input, CancellationToken cancellationToken = default)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)),
            cancellationToken)
        .PipeValue(
            InnerGetUserIdAsync)
        .MapSuccess(
            userId => new ADUserGetOut(id: userId));

    private async ValueTask<Result<Guid, Failure<Unit>>> InnerGetUserIdAsync(
        ADUserGetIn input, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient(httpMessageHandler, disposeHandler: false)
        {
            BaseAddress = apiClientConfiguration.GraphApiBaseAddress
        };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", new(input.AccessToken));

        var response = await httpClient.GetAsync("/v1.0/me?$select=id", cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<UserDataJson>(json)?.Id ?? default;
        }

        return Failure.Create(Invariant($"User data request finished with an unexpected error: {response.StatusCode} {json}"));
    }
}
