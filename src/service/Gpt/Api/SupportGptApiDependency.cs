using GarageGroup.Infra;
using PrimeFuncPack;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.Gpt.Test")]

namespace GarageGroup.Internal.Support;

public static class SupportGptApiDependency
{
    public static Dependency<ISupportGptApi> UseSupportGptApi(this Dependency<IHttpApi, SupportGptApiOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<ISupportGptApi>(CreateApi);

        static SupportGptApi CreateApi(IHttpApi httpApi, SupportGptApiOption option)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            ArgumentNullException.ThrowIfNull(option);

            return new(httpApi, option);
        }
    }
}