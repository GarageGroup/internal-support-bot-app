using System;
using System.Text.Json.Serialization;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed record class AnnotationJsonCreateIn
{
    internal static DataverseEntityCreateIn<AnnotationJsonCreateIn> BuildDataverseCreateInput(
        AnnotationJsonCreateIn annotationJson)
        =>
        new(
            entityPluralName: "annotations",
            entityData: annotationJson);

    internal AnnotationJsonCreateIn(Guid incidentId, string base64Data, string fileName)
    {
        ObjectLookup = $"/incidents({incidentId:D})";
        DocumentBodyBinary = base64Data;
        FileName = fileName;
    }

    [JsonPropertyName("objectid_incident@odata.bind")]
    public string ObjectLookup { get; }

    [JsonPropertyName("documentbody_binary")]
    public string DocumentBodyBinary { get; }

    [JsonPropertyName("filename")]
    public string FileName { get; }

    [JsonPropertyName("subject")]
    public string? Subject { get; init; }
}