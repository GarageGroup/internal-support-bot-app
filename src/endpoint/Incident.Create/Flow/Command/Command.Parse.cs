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

        var description = update.Message.GetDescription();
        var documentIds = update.Message.GetDocumentIds();

        if (string.IsNullOrEmpty(description) && documentIds.IsEmpty)
        {
            return default;
        }

        return new IncidentCreateCommandIn(description)
        {
            DocumentIds = documentIds,
            SourceSender = update.Message.ForwardOrigin switch
            {
                BotMessageOriginUser originUser => originUser.SenderUser,
                _ => null
            }
        };
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
}