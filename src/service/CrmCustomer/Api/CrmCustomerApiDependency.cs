using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmCustomer.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmCustomerApiDependency
{
    public static Dependency<ICrmCustomerApi> UseCrmCustomerApi<TDataverseApi>(this Dependency<TDataverseApi> dependency)
        where TDataverseApi : IDataverseSearchSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ICrmCustomerApi>(CreateApi);

        static CrmCustomerApi CreateApi(TDataverseApi dataverseApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            return new(dataverseApi);
        }
    }
}