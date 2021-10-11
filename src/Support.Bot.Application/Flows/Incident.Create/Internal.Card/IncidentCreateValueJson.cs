using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal sealed record IncidentCreateValueJson
{
    public static readonly IncidentCreateValueJson Create = new()
    {
        ActionName = "Create"
    };

    public static readonly IncidentCreateValueJson Cancel = new()
    {
        ActionName = "Cancel"
    };

    [JsonProperty("actionName")]
    [JsonPropertyName("actionName")]
    public string? ActionName { get; init; }
}

