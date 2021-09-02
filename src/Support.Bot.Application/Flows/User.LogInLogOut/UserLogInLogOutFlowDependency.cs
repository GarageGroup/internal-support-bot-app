using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot
{
    using IUserLogInGetFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;
    using IADUserGetFunc = IAsyncValueFunc<ADUserGetIn, Result<ADUserGetOut, Failure<Unit>>>;
    using IUserGetFunc = IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>;

    public static class UserLogInLogOutFlowDependency
    {
        public static Dependency<IUserLogInGetFlowFunc> UseUserLogInLogOutFlow<TConfiguration>(
            this Dependency<IADUserGetFunc, IUserGetFunc, TConfiguration, UserState, ILoggerFactory> dependency)
            where TConfiguration : IUserLogInConfiguration
            =>
            dependency.Fold<IUserLogInGetFlowFunc>(CreateFlowGetFunc);

        private static UserLogInGetFlowFunc CreateFlowGetFunc<TConfiguration>(
            IADUserGetFunc adUserGetFunc,
            IUserGetFunc userGetFunc,
            TConfiguration userLogInConfiguration,
            UserState userState,
            ILoggerFactory loggerFactory)
            where TConfiguration : IUserLogInConfiguration
            =>
            InnerCreateFlowGetFunc(
                adUserGetFunc ?? throw new ArgumentNullException(nameof(adUserGetFunc)),
                userGetFunc ?? throw new ArgumentNullException(nameof(userGetFunc)),
                userLogInConfiguration ?? throw new ArgumentNullException(nameof(userLogInConfiguration)),
                userState ?? throw new ArgumentNullException(nameof(UserState)),
                loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)));

        private static UserLogInGetFlowFunc InnerCreateFlowGetFunc(
            IADUserGetFunc adUserGetFunc,
            IUserGetFunc userGetFunc,
            IUserLogInConfiguration userLogInConfiguration,
            UserState userState,
            ILoggerFactory loggerFactory)
            =>
            UserLogInGetFlowFunc.InternalCreate(
                userDataAccessor: userState.InternalCreateUserDataAccessor(),
                adUserGetFunc: adUserGetFunc,
                userGetFunc: userGetFunc,
                userLogInConfiguration: userLogInConfiguration,
                logger: loggerFactory.CreateLogger<UserLogInGetFlowFunc>());

        private static IStatePropertyAccessor<UserFlowStateJson?> InternalCreateUserDataAccessor(
            this UserState userState)
            =>
            userState.CreateProperty<UserFlowStateJson?>("UserData");
    }
}