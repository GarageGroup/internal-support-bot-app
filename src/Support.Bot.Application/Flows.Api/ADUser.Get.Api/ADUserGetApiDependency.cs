using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot
{
    using IADUserGetFunc = IAsyncValueFunc<ADUserGetIn, Result<ADUserGetOut, Failure<Unit>>>;

    public static class ADUserGetApiDependency
    {
        public static Dependency<IADUserGetFunc> UseADUserGetApi<TMessageHandler, TApiClientConfiguration>(
            this Dependency<TMessageHandler, TApiClientConfiguration> dependency)
            where TMessageHandler : HttpMessageHandler
            where TApiClientConfiguration : IADUserApiClientConfiguration
            =>
            dependency.Fold<IADUserGetFunc>(
                (h, c) => ADUserGetFunc.Create(h, c));
    }
}