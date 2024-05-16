using Newtonsoft.Json;

namespace GarageGroup.Internal.Support;

internal sealed record class TelegramSenderJson
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }
}