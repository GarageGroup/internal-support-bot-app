using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmContactApi : ICrmContactApi
{
    private static readonly FlatArray<string> ContactSetSearchEntities;

    static CrmContactApi()
        =>
        ContactSetSearchEntities = new("contact");

    private readonly IDataverseSearchSupplier dataverseApi;

    private readonly ISqlQueryEntitySetSupplier sqlApi;

    internal CrmContactApi(IDataverseSearchSupplier dataverseApi, ISqlQueryEntitySetSupplier sqlApi)
    {
        this.dataverseApi = dataverseApi;
        this.sqlApi = sqlApi;
    }
}