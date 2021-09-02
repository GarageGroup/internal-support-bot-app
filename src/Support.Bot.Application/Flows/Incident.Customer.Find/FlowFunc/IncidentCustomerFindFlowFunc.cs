using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot
{
    using IIncidentCustomerFindFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<IncidentCustomerFindFlowOut>>;
    using ICustomerSetFindFunc = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

    internal sealed partial class IncidentCustomerFindFlowFunc : IIncidentCustomerFindFlowFunc
    {
        internal static IncidentCustomerFindFlowFunc InternalCreate(
            ICustomerSetFindFunc customerSetFindFunc, ILogger<IncidentCustomerFindFlowFunc> logger)
            =>
            new(customerSetFindFunc, logger);

        private readonly ICustomerSetFindFunc customerSetFindFunc;

        private readonly ILogger logger;

        private IncidentCustomerFindFlowFunc(ICustomerSetFindFunc customerSetFindFunc, ILogger logger)
        {
            this.customerSetFindFunc = customerSetFindFunc;
            this.logger = logger;
        }
    }
}