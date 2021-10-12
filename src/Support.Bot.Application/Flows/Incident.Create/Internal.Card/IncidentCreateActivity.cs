using GGroupp.Infra;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support.Bot;

internal static class IncidentCreateActivity
{
    public static IActivity CreateConfirmation(IncidentCreateFlowIn input)
        =>
        new HeroCard
        {
            Title = input.Title,
            Subtitle = $"От: {input.CustomerTitle}",
            Text = input.Description,
            Buttons = new CardAction[]
            {
                new(ActionTypes.PostBack)
                {
                    Title = "Создать",
                    Value = IncidentCreateValueJson.Create
                },
                new(ActionTypes.PostBack)
                {
                    Title = "Отменить",
                    Value = IncidentCreateValueJson.Cancel
                }
            }
        }
        .ToAttachment()
        .ToActivity();

    public static IActivity CreateSuccess(IncidentLink incidentLink)
        =>
        new HeroCard
        {
            Title = "Инцидент был создан успешно",
            Buttons = new CardAction[]
            {
                new(ActionTypes.OpenUrl)
                {
                    Title = incidentLink.Title,
                    Value = incidentLink.Url
                }
            }
        }
        .ToAttachment()
        .ToActivity();
}

