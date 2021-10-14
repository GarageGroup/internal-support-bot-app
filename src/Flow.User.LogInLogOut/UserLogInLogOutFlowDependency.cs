using System;
using System.Net.Http;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IUserLogInFlowFunc = IAsyncValueFunc<DialogContext, Unit, ChatFlowStepResult<UserLogInFlowOut>>;
using IADUserGetFunc = IAsyncValueFunc<ADUserGetIn, Result<ADUserGetOut, Failure<Unit>>>;
using IUserGetFunc = IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>;

public static class UserLogInLogOutFlowDependency
{
    public static Dependency<IUserLogInFlowFunc> UseUserLogInFlow<TConfiguration>(
        this Dependency<IADUserGetFunc, IUserGetFunc, TConfiguration, UserState, ILoggerFactory> dependency)
        where TConfiguration : IUserLogInConfiguration
        =>
        dependency.Fold<IUserLogInFlowFunc>(CreateFlowFunc);

    public static Dependency<IADUserGetFunc> UseADUserGetApi<TMessageHandler, TApiClientConfiguration>(
        this Dependency<TMessageHandler, TApiClientConfiguration> dependency)
        where TMessageHandler : HttpMessageHandler
        where TApiClientConfiguration : IADUserApiClientConfiguration
        =>
        dependency.Fold<IADUserGetFunc>(
            (h, c) => ADUserGetFunc.Create(h, c));

    private static UserLogInGetFlowFunc CreateFlowFunc<TConfiguration>(
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
