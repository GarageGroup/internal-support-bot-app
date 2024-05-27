using GarageGroup.Infra;
using System;

namespace GarageGroup.Internal.Support;

using TDataverseApi = IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>;

internal sealed partial class CrmIncidentApi(IHttpApi httpApi, TDataverseApi dataverseApi) : ICrmIncidentApi
{
    private const string PictureSubject = "Picture from user";

    private sealed record class AnnotationInput(PictureModel Picture, Guid CallerUserId, Guid IncidentId);

    private sealed record class AnnotationSetInput(FlatArray<PictureModel> Pictures, Guid CallerUserId, IncidentCreateOut Incident);
}