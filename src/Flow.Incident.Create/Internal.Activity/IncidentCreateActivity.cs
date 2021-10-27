using System;
using System.Collections.Generic;
using System.Text;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal static class IncidentCreateActivity
{
    private const string ActionCreate = "Создать";

    private const string ActionCancel = "Отменить";

    private static readonly IReadOnlyDictionary<int, string> typeCodes;

    static IncidentCreateActivity()
        => typeCodes = new Dictionary<int, string>()
        {
            [1] = "Вопрос",
            [2] = "Проблема",
            [3] = "Запрос"
        };

    public static bool IsConfirmed(this Activity activity)
    {
        if (activity.Type != ActivityTypes.Message)
        {
            return false;
        }

        if (activity.IsCardSupported())
        {
            var json = activity.Value.ToStringOrEmpty();
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            var value = JsonConvert.DeserializeObject<ConfirmtionValueJson>(json);
            return IsActionCreate(value?.Command);
        }

        return IsActionCreate(activity.Text);

        static bool IsActionCreate(string? text)
            =>
            string.Equals(text, ActionCreate, StringComparison.InvariantCultureIgnoreCase);
    }

    public static IActivity CreateConfirmationActivity(this Activity activity, IncidentCreateFlowIn input)
    {
        var card = activity.CreateConfirmationCard(input);
        if (activity.IsCardSupported())
        {
            return card.ToAttachment().ToActivity();
        }

        var textBuilder = new StringBuilder($"Заголовок: {input.Title}");
        if (string.IsNullOrEmpty(input.CustomerTitle) is false)
        {
            textBuilder = textBuilder.Append($"\n\r\n\rКлиент: {input.CustomerTitle}");
        }
        textBuilder = textBuilder.Append($"\n\r\n\rОписание: {input.Description}");
        textBuilder = textBuilder.Append($"\n\r\n\rТип обращения: {input.CaseTypeCode.MapCaseTypeCode()}");

        card.Title = "Создать инцидент?";
        card.Subtitle = null;
        card.Text = null;

        return MessageFactory.Attachment(card.ToAttachment(), textBuilder.ToString());
    }

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

    private static HeroCard CreateConfirmationCard(this Activity activity, IncidentCreateFlowIn input)
        =>
        new()
        {
            Title = input.Title,
            Subtitle = $"Клиент: {input.CustomerTitle}<br>Тип обращения: {input.CaseTypeCode.MapCaseTypeCode()}",
            Text = input.Description,
            Buttons = new CardAction[]
            {
                new(ActionTypes.PostBack)
                {
                    Title = "Создать",
                    Text = ActionCreate,
                    Value = CreateButtonValue(activity, ActionCreate)
                },
                new(ActionTypes.PostBack)
                {
                    Title = "Отменить",
                    Text = ActionCancel,
                    Value = CreateButtonValue(activity, ActionCancel)
                }
            }
        };

    private static object CreateButtonValue(Activity activity, string commandName)
        => 
        activity.IsCardSupported() ? new ConfirmtionValueJson { Command = commandName } : commandName;

    private static string MapCaseTypeCode(this int code)
        => typeCodes.ContainsKey(code) ? typeCodes[code] : throw new ArgumentOutOfRangeException(nameof(code));

    private sealed record ConfirmtionValueJson
    {
        [JsonProperty("customerId")]
        public string? Command { get; init; }
    }
}

