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

    internal static string BuildOwnerLookupValue(Guid ownerId)
        =>
        $"/systemusers({ownerId:D})";

    [JsonPropertyName("objectidtypecode")]
    public string? ObjectIdTypeCode { get; init; }

    [JsonPropertyName("objectid")]
    public string? ObjectId { get; init; }

    [JsonPropertyName("documentbody_binary")]
    public byte[]? DocumentBodyBinary { get; init; }
}