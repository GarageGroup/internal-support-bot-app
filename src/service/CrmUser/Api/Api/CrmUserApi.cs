using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmUserApi : ICrmUserApi
{
    private static readonly FlatArray<string> UserSetSearchEntities;

    static CrmUserApi()
        =>
        UserSetSearchEntities = new("systemuser");

    private readonly IDataverseSearchSupplier dataverseApi;

    internal CrmUserApi(IDataverseSearchSupplier dataverseApi)
        =>
        this.dataverseApi = dataverseApi;
}