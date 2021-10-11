using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal sealed record IncidentCreateDataJson
{
    public static readonly IncidentCreateDataJson Create = new()
    {
        ActionName = "Create"
    };

    public static readonly IncidentCreateDataJson Cancel = new()
    {
        ActionName = "Cancel"
    };

    [JsonProperty("actionName")]
    [JsonPropertyName("actionName")]
    public string? ActionName { get; init; }
}

