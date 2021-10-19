using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GGroupp.Internal.Support.Bot;

internal static class IncidentTypeActivity
{
    public static IActivity CreateTypeHintActivity(this Activity activity, IReadOnlyDictionary<int, string> caseCodeTypes)
        =>
        new HeroCard
        {
            Title = "Выберите тип обращения",
            Buttons = caseCodeTypes.Select(activity.CreateCaseCodeAction).ToArray()
        }
        .ToAttachment()
        .ToActivity();

    public static Result<int, Failure<Unit>> GetTypeValueOrFailure(
        this Activity activity,
        IReadOnlyCollection<int> awailableCodeTypes)
    {
        if (activity.IsMessageType() is false)
        {
            return Failure.Create(string.Empty);
        }

        if (activity.IsCardSupported())
        {
            var json = activity.Value.ToStringOrEmpty();
            if (string.IsNullOrEmpty(json) is false)
            {
                var value = JsonConvert.DeserializeObject<HintValueJson>(json);
                return value.CaseCode.AwailableOrFailure(awailableCodeTypes);
            }
        }

        return activity.ParseTypeValueOrFailure().Forward(v => v.AwailableOrFailure(awailableCodeTypes));
    }

    private static Result<int, Failure<Unit>> ParseTypeValueOrFailure(this Activity activity)
        =>
        int.TryParse(activity.Text, out var value) 
            ? value 
            : Failure.Create("Не удалось распарсить значение");

    private static Result<int, Failure<Unit>> AwailableOrFailure(this int value, IReadOnlyCollection<int> awailableValues)
        =>
        awailableValues.Contains(value) ? value : Failure.Create("Это значение недопустимо");

    private sealed record HintValueJson
    {
        [JsonProperty("casecode")]
        public int CaseCode { get; init; }
    }

    private static CardAction CreateCaseCodeAction(this Activity activity, KeyValuePair<int, string> value)
        =>
        new(ActionTypes.PostBack)
        {
            Title = value.Value,
            Text = value.Value,
            Value = activity.IsCardSupported()
                ? new HintValueJson() { CaseCode = value.Key }
                : value.Key
        };
}

