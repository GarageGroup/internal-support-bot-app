using System;
using System.Collections.Generic;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateCommandExtensions
{
    internal static FlatArray<string> GetDocumentIds(this BotMessage message)
    {
        return InnerGetDocumentIds(message).ToFlatArray();

        static IEnumerable<string> InnerGetDocumentIds(BotMessage message)
        {
            if (message.Photo.IsNotEmpty)
            {
                yield return message.Photo[^1].FileId;
            }

            if (message.Document is not null)
            {
                yield return message.Document.FileId;
            }

            if (message.Video is not null)
            {
                yield return message.Video.FileId;
            }
        }
    }
}