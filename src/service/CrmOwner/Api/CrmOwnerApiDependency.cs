using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmOwner.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmOwnerApiDependency
{
    public static Dependency<ICrmOwnerApi> UseCrmOwnerApi<TDataverseApi, TSqlApi>(
        this Dependency<TDataverseApi, TSqlApi> dependency)
        where TDataverseApi : IDataverseSearchSupplier
        where TSqlApi : ISqlQueryEntitySetSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<ICrmOwnerApi>(CreateApi);

        static CrmOwnerApi CreateApi(TDataverseApi dataverseApi, TSqlApi sqlApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            ArgumentNullException.ThrowIfNull(sqlApi);

            return new(dataverseApi, sqlApi);
        }
    }
}