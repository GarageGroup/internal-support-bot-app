using Newtonsoft.Json;

namespace GarageGroup.Internal.Support;

internal sealed record class TelegramSenderState
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("firstName")]
    public string? FirstName { get; set; }

    [JsonProperty("lastName")]
    public string? LastName { get; set; }
}