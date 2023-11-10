using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmOwnerApi : ICrmOwnerApi
{
    private static readonly FlatArray<string> OwnerSetSearchEntities;

    static CrmOwnerApi()
        =>
        OwnerSetSearchEntities = new("systemuser");

    private readonly IDataverseSearchSupplier dataverseApi;

    internal CrmOwnerApi(IDataverseSearchSupplier dataverseApi)
        =>
        this.dataverseApi = dataverseApi;
}