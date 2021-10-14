using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support.Bot;

internal sealed record UserDataJson
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
}