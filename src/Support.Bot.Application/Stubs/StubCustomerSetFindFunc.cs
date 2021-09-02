namespace GGroupp.Internal.Support.Bot
{
    using System.Threading;
    using System.Threading.Tasks;
    using ICustomerSetFindFunc = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

    internal sealed class StubCustomerSetFindFunc : ICustomerSetFindFunc
    {
        public static readonly StubCustomerSetFindFunc Instance = new();

        private StubCustomerSetFindFunc()
        {
        }

        public ValueTask<Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>> InvokeAsync(
            CustomerSetFindIn input, CancellationToken cancellationToken = default)
            =>
            AsyncPipeline.Start(
                input ?? throw new ArgumentNullException(nameof(input)),
                cancellationToken)
            .Pipe<Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>(
                @in => string.Equals(input.SearchText, "Test", StringComparison.InvariantCultureIgnoreCase)
                    ? new CustomerSetFindOut(CreateStubCustomers())
                    : Failure.Create(CustomerSetFindFailureCode.NotFound, "Any customers were not found."));

        private static IReadOnlyCollection<CustomerItemFindOut> CreateStubCustomers()
            =>
            new CustomerItemFindOut[]
            {
                new(Guid.NewGuid(), "First customer"),
                new(Guid.NewGuid(), "Second customer"),
                new(Guid.NewGuid(), "Third customer")
            };
    }
}