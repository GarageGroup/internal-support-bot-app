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
    /*new AdaptiveCardJson("1.3")
    {
        Type = "AdaptiveCard",
        Body = new object[]
        {
            new
            {
                type = "TextBlock",
                size = "Medium",
                weight = "Bolder",
                text = input.Title
            },
            new
            {
                type = "ColumnSet",
                columns = new object[]
                {
                    new
                    {
                        type = "Column",
                        items = new object[]
                        {
                            new
                            {
                                type = "TextBlock",
                                text = "От:"
                            }
                        },
                        width = "auto"
                    },
                    new
                    {
                        type = "Column",
                        items = new object[]
                        {
                            new
                            {
                                type = "TextBlock",
                                text = input.CustomerTitle,
                                weight = "Bolder"
                            }
                        },
                        width = "auto"
                    }
                }
            },
            new
            {
                type = "TextBlock",
                wrap = true,
                text = input.Description
            }
        },
        Actions = new object[]
        {
            new
            {
                type = "Action.Submit",
                title = "Создать",
                data = IncidentCreateValueJson.Create
            },
            new
            {
                type = "Action.Submit",
                title = "Отменить",
                data = IncidentCreateValueJson.Cancel
            }
        }
    }
    .Pipe(
        dialogContext.Context.Activity.CreateReplyFromCard);*/

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
    /*new AdaptiveCardJson("1.3")
    {
        Type = "AdaptiveCard",
        Body = new object[]
        {
            new
            {
                type = "TextBlock",
                size = "Medium",
                weight = "Bolder",
                text = "Инцидент был создан успешно"
            }
        },
        Actions = new object[]
        {
            new
            {
                type = "Action.OpenUrl",
                title = incidentLink.Title,
                url = incidentLink.Url
            }
        }
    }
    .Pipe(
        dialogContext.Context.Activity.CreateReplyFromCard);*/
}

