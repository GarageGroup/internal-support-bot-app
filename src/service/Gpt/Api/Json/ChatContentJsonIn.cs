using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.Json;

internal sealed record class ChatContentJsonIn
{
    public ChatContentJsonIn(string? text)
    {
        Type = "text";
        Text = text.OrEmpty();
    }

    public ChatContentJsonIn(ChatImageUrlJsonIn image)
    {
        Type = "image_url";
        Image = image;
    }

    [JsonPropertyName("type")]
    public string? Type { get; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; }

    [JsonPropertyName("image_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ChatImageUrlJsonIn? Image { get; }
}