using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateCommandExtensions
{
    internal static string? GetDescription(this BotMessage message)
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