using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmIncident.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmIncidentApiDependency
{
    public static Dependency<ICrmIncidentApi> UseCrmIncidentApi<TDataverseApi>(this Dependency<TDataverseApi> dependency)
        where TDataverseApi : IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ICrmIncidentApi>(CreateApi);

        static CrmIncidentApi CreateApi(TDataverseApi dataverseApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            return new(dataverseApi);
        }
    }
}