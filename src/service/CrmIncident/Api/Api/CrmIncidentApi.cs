using GarageGroup.Infra;
using System;

namespace GarageGroup.Internal.Support;

internal sealed partial class CrmIncidentApi(IHttpApi httpApi, IDataverseEntityCreateSupplier dataverseApi) : ICrmIncidentApi
{
    private const string PictureSubject = "Picture from user";

    private const string DocumentSubject = "Document from user";

    private const string VideoSubject = "Video from user";

    private sealed record class AnnotationInput(DocumentModel Document, Guid CallerObjectId, Guid IncidentId);

    private sealed record class AnnotationSetInput(FlatArray<DocumentModel> Documents, Guid CallerObjectId, IncidentCreateOut Incident);
}