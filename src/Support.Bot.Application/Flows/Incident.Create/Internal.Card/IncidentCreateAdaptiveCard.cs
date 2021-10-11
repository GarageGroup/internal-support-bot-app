using System;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support.Bot;

internal static class IncidentCreateAdaptiveCard
{
    public static IActivity CreateConfirmationActivity(this DialogContext dialogContext, IncidentCreateFlowIn input)
        =>
        new AdaptiveCardJson("1.3")
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
                    data = IncidentCreateDataJson.Create
                },
                new
                {
                    type = "Action.Submit",
                    title = "Отменить",
                    data = IncidentCreateDataJson.Cancel
                }
            }
        }
        .Pipe(
            dialogContext.Context.Activity.CreateReplyFromCard);

    public static IActivity CreateSuccessActivity(this DialogContext dialogContext, IncidentLink incidentLink)
        =>
        new AdaptiveCardJson("1.3")
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
            dialogContext.Context.Activity.CreateReplyFromCard);
}

