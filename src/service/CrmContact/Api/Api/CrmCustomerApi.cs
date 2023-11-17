using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmContactApi(IDataverseSearchSupplier dataverseApi, ISqlQueryEntitySetSupplier sqlApi) : ICrmContactApi
{
    private static readonly FlatArray<string> ContactSetSearchEntities;

    static CrmContactApi()
        =>
        ContactSetSearchEntities = new("contact");
}