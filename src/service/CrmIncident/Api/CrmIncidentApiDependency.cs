using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmIncident.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmIncidentApiDependency
{
    public static Dependency<ICrmIncidentApi> UseCrmIncidentApi<TDataverseApi>(this Dependency<IHttpApi, TDataverseApi> dependency)
        where TDataverseApi : IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<ICrmIncidentApi>(CreateApi);

        static CrmIncidentApi CreateApi(IHttpApi httpApi, TDataverseApi dataverseApi)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            ArgumentNullException.ThrowIfNull(dataverseApi);

            return new(httpApi, dataverseApi);
        }
    }
}