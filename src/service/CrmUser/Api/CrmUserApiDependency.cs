using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmUser.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmUserApiDependency
{
    public static Dependency<ICrmUserApi> UseCrmUserApi<TDataverseApi>(this Dependency<TDataverseApi> dependency)
        where TDataverseApi : IDataverseSearchSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ICrmUserApi>(CreateApi);

        static CrmUserApi CreateApi(TDataverseApi dataverseApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            return new(dataverseApi);
        }
    }
}