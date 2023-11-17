using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmOwnerApi(IDataverseSearchSupplier dataverseApi, ISqlQueryEntitySetSupplier sqlApi) : ICrmOwnerApi
{
    private static readonly FlatArray<string> OwnerSetSearchEntities;

    static CrmOwnerApi()
        =>
        OwnerSetSearchEntities = new("systemuser");
}