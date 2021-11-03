using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal static class IncidentTitleActivity
{
    public static IActivity CreateTitleHintActivity(this Activity activity, string title)
    {
        var card = activity.CreateTitleHintCard(title);
        if (activity.IsCardSupported())
        {
            return card.ToAttachment().ToActivity();
        }

        var cardTitle = card.Title;

        card.Title = card.Text;
        card.Text = null;

        return MessageFactory.Attachment(card.ToAttachment(), cardTitle);
    }

    public static string GetTitleValue(this Activity activity)
    {
        if (activity.Type != ActivityTypes.Message)
        {
            return string.Empty;
        }

        if (activity.IsCardSupported())
        {
            var json = activity.Value.ToStringOrEmpty();
            if (string.IsNullOrEmpty(json) is false)
            {
                var value = JsonConvert.DeserializeObject<HintValueJson>(json);
                return value.Title.OrEmpty();
            }
        }

        return activity.Text.OrEmpty();
    }

    private static HeroCard CreateTitleHintCard(this Activity activity, string title)
        =>
        new()
        {
            Title = "Теперь нужно указать заголовок",
            Text = "Можно воспользовать предложенным или ввести свой",
            Buttons = new CardAction[]
            {
                new(ActionTypes.PostBack)
                {
                    Title = title,
                    Text = title,
                    Value = activity.IsCardSupported() ? new HintValueJson { Title = title } : title
                }
            }
        };

    private sealed record HintValueJson
    {
        [JsonProperty("title")]
        public string? Title { get; init; }
    }
}

