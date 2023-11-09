using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmIncidentApi<TDataverseApi> : ICrmIncidentApi
    where TDataverseApi : IDataverseEntityCreateSupplier, IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>
{
    private readonly TDataverseApi dataverseApi;

    internal CrmIncidentApi(TDataverseApi dataverseApi)
        =>
        this.dataverseApi = dataverseApi;

    private IDataverseEntityCreateSupplier GetDataverseApi(Guid? callerUserId)
        =>
        callerUserId switch
        {
            null => dataverseApi,
            _ => dataverseApi.Impersonate(callerUserId.Value)
        };
}