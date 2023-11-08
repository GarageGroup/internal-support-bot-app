using System;

namespace GarageGroup.Internal.Support;

public sealed record class ChatMessageOption
{
    public ChatMessageOption(string role, string contentTemplate)
    {
        Role = role.OrEmpty();
        ContentTemplate = contentTemplate.OrEmpty();
    }

    public string Role { get; }

    public string ContentTemplate { get; }
}