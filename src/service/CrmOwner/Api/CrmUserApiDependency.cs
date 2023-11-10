using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmOwner.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmOwnerApiDependency
{
    public static Dependency<ICrmOwnerApi> UseCrmOwnerApi<TDataverseApi>(this Dependency<TDataverseApi> dependency)
        where TDataverseApi : IDataverseSearchSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ICrmOwnerApi>(CreateApi);

        static CrmOwnerApi CreateApi(TDataverseApi dataverseApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            return new(dataverseApi);
        }
    }
}