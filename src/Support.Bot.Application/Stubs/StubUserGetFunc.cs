namespace GGroupp.Internal.Support.Bot
{
    internal sealed class StubUserGetFunc : IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>
    {
        public static readonly StubUserGetFunc Instance = new();

        private StubUserGetFunc()
        {
        }

        public ValueTask<Result<UserGetOut, Failure<UserGetFailureCode>>> InvokeAsync(
            UserGetIn input, CancellationToken cancellationToken = default)
            =>
            AsyncPipeline.Start(
                input ?? throw new ArgumentNullException(nameof(input)),
                cancellationToken)
            .Pipe<Result<UserGetOut, Failure<UserGetFailureCode>>>(
                @in => new UserGetOut(systemUserId: @in.ActiveDirectoryUserId));
    }
}