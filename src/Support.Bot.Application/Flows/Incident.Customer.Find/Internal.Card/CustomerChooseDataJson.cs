using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal sealed record IncidentCreateActionDataJson
{
    public static readonly IncidentCreateActionDataJson Create = new()
    {
        ActionName = "Create"
    };

    public static readonly IncidentCreateActionDataJson Cancel = new()
    {
        ActionName = "Cancel"
    };

    [JsonProperty("actionName")]
    [JsonPropertyName("actionName")]
    public string? ActionName { get; init; }
}

