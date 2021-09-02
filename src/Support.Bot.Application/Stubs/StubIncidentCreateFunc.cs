namespace GGroupp.Internal.Support.Bot
{
    using System.Threading;
    using System.Threading.Tasks;
    using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

    internal sealed class StubIncidentCreateFunc : IIncidentCreateFunc
    {
        public static readonly StubIncidentCreateFunc Instance = new();

        private StubIncidentCreateFunc()
        {
        }

        public ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
            IncidentCreateIn input, CancellationToken cancellationToken = default)
            =>
            AsyncPipeline.Start(
                input ?? throw new ArgumentNullException(nameof(input)),
                cancellationToken)
            .Pipe<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>(
                @in => new IncidentCreateOut(
                    id: Guid.NewGuid(),
                    title: @in.Title));
    }
}