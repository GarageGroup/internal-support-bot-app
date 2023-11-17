using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmCustomerApi(IDataverseSearchSupplier dataverseApi, ISqlQueryEntitySetSupplier sqlApi) : ICrmCustomerApi
{
    private static readonly FlatArray<string> CustomerSetSearchEntities;

    static CrmCustomerApi()
        =>
        CustomerSetSearchEntities = new("account");
}