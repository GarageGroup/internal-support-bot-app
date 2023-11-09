using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmContact.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmContactApiDependency
{
    public static Dependency<ICrmContactApi> UseCrmContactApi<TDataverseApi>(this Dependency<TDataverseApi> dependency)
        where TDataverseApi : IDataverseSearchSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ICrmContactApi>(CreateApi);

        static CrmContactApi CreateApi(TDataverseApi dataverseApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            return new(dataverseApi);
        }
    }
}