using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmContact.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmContactApiDependency
{
    public static Dependency<ICrmContactApi> UseCrmContactApi<TDataverseApi, TSqlApi>(
        this Dependency<TDataverseApi, TSqlApi> dependency)
        where TDataverseApi : IDataverseSearchSupplier
        where TSqlApi : ISqlQueryEntitySupplier, ISqlQueryEntitySetSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<ICrmContactApi>(CreateApi);

        static CrmContactApi CreateApi(TDataverseApi dataverseApi, TSqlApi sqlApi)
        {
            ArgumentNullException.ThrowIfNull(dataverseApi);
            ArgumentNullException.ThrowIfNull(sqlApi);

            return new(dataverseApi, sqlApi, sqlApi);
        }
    }
}