using System;
using System.Net.Http;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

public static class SupportGptApiDependency
{
    public static Dependency<ISupportGptApi> UseSupportGptApi(this Dependency<HttpMessageHandler, SupportGptApiOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<ISupportGptApi>(CreateApi);

        static SupportGptApi CreateApi(HttpMessageHandler httpMessageHandler, SupportGptApiOption option)
        {
            ArgumentNullException.ThrowIfNull(httpMessageHandler);
            ArgumentNullException.ThrowIfNull(option);

            return new(httpMessageHandler, option);
        }
    }
}