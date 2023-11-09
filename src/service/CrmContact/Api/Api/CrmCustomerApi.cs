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

    internal CrmContactApi(IDataverseSearchSupplier dataverseApi)
        =>
        this.dataverseApi = dataverseApi;
}