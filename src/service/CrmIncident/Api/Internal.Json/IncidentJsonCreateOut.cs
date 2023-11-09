using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal readonly record struct IncidentJsonCreateOut
{
    private const string FieldTitle = "title";

    private const string FieldIncidentId = "incidentid";

    internal static readonly FlatArray<string> SelectedFields;

    static IncidentJsonCreateOut()
        =>
        SelectedFields = new(FieldTitle);

    [JsonPropertyName(FieldIncidentId)]
    public Guid IncidentId { get; init; }

    [JsonPropertyName(FieldTitle)]
    public string? Title { get; init; }
}