using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace GGroupp.Infra;

public static class ActivityExtensions
{
    public static Result<TJson, Unit> GetDeserializedValue<TJson>(this Activity activity)
        =>
        InnerGetDeserializedValue<TJson>(
            activity ?? throw new ArgumentNullException(nameof(activity)));

    public static IMessageActivity ToActivity(this Attachment attachment)
        =>
        MessageFactory.Attachment(
            attachment ?? throw new ArgumentNullException(nameof(attachment)));

    private static Result<TJson, Unit> InnerGetDeserializedValue<TJson>(this Activity activity)
    {
        if (activity.Type == ActivityTypes.Message)
        {
            var jsonValue = activity.Value.ToStringOrEmpty();
            if (string.IsNullOrEmpty(jsonValue) is false)
            {
                return JsonConvert.DeserializeObject<TJson>(jsonValue);
            }
        }

        return default;
    }
}