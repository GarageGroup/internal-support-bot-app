using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmCustomer.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmCustomerApiDependency
{
    public static Dependency<ICrmCustomerApi> UseCrmCustomerApi<TDataverseApi, TSqlApi>(
        this Dependency<TDataverseApi, TSqlApi> dependency)
        where TDataverseApi : IDataverseSearchSupplier
        where TSqlApi : ISqlQueryEntitySetSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<ICrmCustomerApi>(CreateApi);

        static CrmCustomerApi CreateApi(TDataverseApi dataverseApi, TSqlApi sqlApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            ArgumentNullException.ThrowIfNull(sqlApi);

            return new(dataverseApi, sqlApi);
        }
    }
}