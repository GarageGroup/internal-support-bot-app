using GarageGroup.Infra;
using System;

namespace GarageGroup.Internal.Support;

using TDataverseApi = IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>;

internal sealed partial class CrmIncidentApi(IHttpApi httpApi, TDataverseApi dataverseApi) : ICrmIncidentApi
{
    private const string PictureSubject = "Picture from user";

    private const string DocumentSubject = "Document from user";

    private const string VideoSubject = "Video from user";

    private sealed record class AnnotationInput(DocumentModel Document, Guid CallerUserId, Guid IncidentId);

    private sealed record class AnnotationSetInput(FlatArray<DocumentModel> Documents, Guid CallerUserId, IncidentCreateOut Incident);
}