using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmCustomerApi : ICrmCustomerApi
{
    private static readonly FlatArray<string> CustomerSetSearchEntities;

    static CrmCustomerApi()
        =>
        CustomerSetSearchEntities = new("account");

    private readonly IDataverseSearchSupplier dataverseApi;

    private readonly ISqlQueryEntitySetSupplier sqlApi;

    internal CrmCustomerApi(IDataverseSearchSupplier dataverseApi, ISqlQueryEntitySetSupplier sqlApi)
    {
        this.dataverseApi = dataverseApi;
        this.sqlApi = sqlApi;
    }
}