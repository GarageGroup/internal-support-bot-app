using System;
using System.Net.Http;
using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

using IADUserGetFunc = IAsyncValueFunc<ADUserGetIn, Result<ADUserGetOut, Failure<Unit>>>;
using IUserGetFunc = IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;
using ICustomerSetFind = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

internal static class BotDependencyApi
{
    public static Dependency<IUserGetFunc> UseUserGetApi()
        =>
        UseDataverseApiClient("UserGetApi").UseUserGetApi();

    public static Dependency<IIncidentCreateFunc> UseIncidentCreateApi()
        =>
        UseDataverseApiClient("IncidentCreateApi").UseIncidentCreateApi();

    public static Dependency<ICustomerSetFind> UseCustomerSetFindApi()
        =>
        UseDataverseApiClient("CustomerSetFindApi").UseCustomerSetFindApi();

    public static Dependency<IADUserGetFunc> UseADUserGetApi()
        =>
        UseStandardHttpMessageHandler("ADUserGetApi")
        .With(
            BotServiceProvider.GetConfiguration<ADUserApiClientConfiguration>)
        .UseADUserGetApi();

    private static Dependency<IDataverseApiClient> UseDataverseApiClient(string loggerCategoryName)
        =>
        UseStandardHttpMessageHandler(loggerCategoryName)
        .With(
            BotServiceProvider.GetConfiguration<DataverseApiClientConfiguration>)
        .UseDataverseApiClient();

    private static Dependency<HttpMessageHandler> UseStandardHttpMessageHandler(string loggerCategoryName)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .Map<HttpMessageHandler>(
            (sp, handler) => LoggerDelegatingHandler.Create(
                sp.GetLogger(loggerCategoryName), handler));
}
