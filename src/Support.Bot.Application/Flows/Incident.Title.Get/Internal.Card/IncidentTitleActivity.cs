using System;
using GGroupp.Infra;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal static class IncidentTitleActivity
{
    public static IActivity CreateTitleHintActivity(this Activity activity, string title)
        =>
        new HeroCard
        {
            Title = "Теперь нужно указать заголовок",
            Text = activity.IsCardSupported() ? "Можно воспользовать предложенным или ввести свой" : null,
            Buttons = new CardAction[]
            {
                new(ActionTypes.PostBack)
                {
                    Title = title,
                    Text = title,
                    Value = activity.IsCardSupported() ? new HintValueJson { Title = title } : title
                }
            }
        }
        .ToAttachment()
        .ToActivity();

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

    private sealed record HintValueJson
    {
        [JsonProperty("title")]
        public string? Title { get; init; }
    }
}

