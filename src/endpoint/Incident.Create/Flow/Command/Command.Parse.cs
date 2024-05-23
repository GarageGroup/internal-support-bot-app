using System;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateCommand
{
    public Optional<IncidentCreateCommandIn> Parse(ChatUpdate update)
    {
        if (update.Message is null || IsOnlyCommandName(update.Message))
        {
            return default;
        }

        var description = GetDescription(update.Message);
        if (string.IsNullOrEmpty(description))
        {
            return default;
        }

        return new IncidentCreateCommandIn(description)
        {
            PhotoIdSet = update.Message.Photo.Map(GetFileId),
            SourceSender = update.Message.ForwardOrigin switch
            {
                BotMessageOriginUser originUser => originUser.SenderUser,
                _ => null
            }
        };

        static string GetFileId(BotPhotoSize photo)
            =>
            photo.FileId;
    }

    private static bool IsOnlyCommandName(BotMessage message)
    {
        if (message.Entities.Length is not 1)
        {
            return false;
        }

        var entity = message.Entities[0];
        if (entity.Type is not BotMessageEntityType.BotCommand)
        {
            return default;
        }

        return message.Text?.Length == entity.Length;
    }

    private static string? GetDescription(BotMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.Text) is false)
        {
            return message.Text;
        }

        if (string.IsNullOrWhiteSpace(message.Caption) is false)
        {
            return message.Caption;
        }

        return default;
    }
}