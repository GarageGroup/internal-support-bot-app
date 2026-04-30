using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Support.Service.CrmProject.Test")]

namespace GarageGroup.Internal.Support;

public static class CrmProjectApiDependency
{
    public static Dependency<ICrmProjectApi> UseCrmProjectApi<TSqlApi>(this Dependency<TSqlApi> dependency)
        where TSqlApi : ISqlQueryEntitySetSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ICrmProjectApi>(CreateApi);

        static CrmProjectApi CreateApi(TSqlApi sqlApi)
        {
            ArgumentNullException.ThrowIfNull(sqlApi);

            return new(sqlApi);
        }
    }
}