using System;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateCommandExtensions
{
    internal static FlatArray<string> GetPhotoIds(this BotMessage message)
    {
        if (message.Photo.IsEmpty)
        {
            return default;
        }

        return [message.Photo[^1].FileId];
    }
}