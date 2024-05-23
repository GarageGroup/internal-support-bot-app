using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmContactApi : ICrmContactApi
{
    private static readonly FlatArray<string> ContactSetSearchEntities
        =
        new("contact");

    private readonly IDataverseSearchSupplier dataverseApi;

    private readonly ISqlQueryEntitySupplier sqlEntityApi;

    private readonly ISqlQueryEntitySetSupplier sqlEntitySetApi;

    internal CrmContactApi(IDataverseSearchSupplier dataverseApi, ISqlQueryEntitySupplier sqlEntityApi, ISqlQueryEntitySetSupplier sqlEntitySetApi)
    {
        this.dataverseApi = dataverseApi;
        this.sqlEntityApi = sqlEntityApi;
        this.sqlEntitySetApi = sqlEntitySetApi;
    }
}