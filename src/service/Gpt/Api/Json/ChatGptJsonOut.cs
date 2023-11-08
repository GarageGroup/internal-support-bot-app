using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal readonly record struct ChatGptJsonOut
{
    [JsonPropertyName("choices")]
    public FlatArray<ChatChoiceJson> Choices { get; init; }
}