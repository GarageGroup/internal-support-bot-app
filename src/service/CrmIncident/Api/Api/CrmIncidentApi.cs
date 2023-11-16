using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

using TDataverseApi = IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>;

internal sealed partial class CrmIncidentApi(TDataverseApi dataverseApi) : ICrmIncidentApi
{
}