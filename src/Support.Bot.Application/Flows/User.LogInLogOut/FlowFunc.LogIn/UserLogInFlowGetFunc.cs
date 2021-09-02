using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot
{
    using IUserLogInGetFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;
    using IADUserGetFunc = IAsyncValueFunc<ADUserGetIn, Result<ADUserGetOut, Failure<Unit>>>;
    using IUserGetFunc = IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>;

    internal sealed partial class UserLogInGetFlowFunc : IUserLogInGetFlowFunc
    {
        internal static UserLogInGetFlowFunc InternalCreate(
            IStatePropertyAccessor<UserFlowStateJson?> userDataAccessor,
            IUserLogInConfiguration userLogInConfiguration,
            IADUserGetFunc adUserGetFunc,
            IUserGetFunc userGetFunc,
            ILogger<UserLogInGetFlowFunc> logger)
            =>
            new(userDataAccessor, adUserGetFunc, userGetFunc, userLogInConfiguration, logger);

        private readonly IStatePropertyAccessor<UserFlowStateJson?> userDataAccessor;

        private readonly IADUserGetFunc adUserGetFunc;

        private readonly IUserGetFunc userGetFunc;

        private readonly IUserLogInConfiguration userLogInConfiguration;

        private readonly ILogger logger;

        private UserLogInGetFlowFunc(
            IStatePropertyAccessor<UserFlowStateJson?> userDataAccessor,
            IADUserGetFunc adUserGetFunc,
            IUserGetFunc userGetFunc,
            IUserLogInConfiguration userLogInConfiguration,
            ILogger<UserLogInGetFlowFunc> logger)
        {
            this.userDataAccessor = userDataAccessor;
            this.adUserGetFunc = adUserGetFunc;
            this.userGetFunc = userGetFunc;
            this.userLogInConfiguration = userLogInConfiguration;
            this.logger = logger;
        }
    }
}