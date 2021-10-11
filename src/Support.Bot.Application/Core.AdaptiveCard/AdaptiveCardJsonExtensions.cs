using System;
using System.Collections.Generic;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace GGroupp.Infra;

public static class AdaptiveCardJsonExtensions
{
    public static Result<TAdaptiveResponse, Unit> GetAdaptiveResponse<TAdaptiveResponse>(this Activity activity)
        =>
        InnerGetAdaptiveResponse<TAdaptiveResponse>(
            activity ?? throw new ArgumentNullException(nameof(activity)));

    public static Activity CreateReplyFromCard(this Activity originalActivity, AdaptiveCardJson adaptiveCard)
        =>
        InnerCreateReplyFromCard(
            originalActivity ?? throw new ArgumentNullException(nameof(originalActivity)),
            adaptiveCard ?? throw new ArgumentNullException(nameof(adaptiveCard)));

    private static Result<TAdaptiveResponse, Unit> InnerGetAdaptiveResponse<TAdaptiveResponse>(this Activity activity)
    {
        if (activity.Type == ActivityTypes.Message)
        {
            var jsonValue = activity.Value.ToStringOrEmpty();
            if (string.IsNullOrEmpty(jsonValue) is false)
            {
                return JsonConvert.DeserializeObject<TAdaptiveResponse>(jsonValue);
            }
        }

        return default;
    }

    private static Activity InnerCreateReplyFromCard(Activity originalActivity, AdaptiveCardJson adaptiveCard)
    {
        var reply = new Activity
        {
            Attachments = new List<Attachment>()
        };

        reply.Attachments.Add(adaptiveCard.ToAttachement());
        originalActivity.SetReplyFields(reply);

        return reply;
    }

    private static Attachment ToAttachement(this AdaptiveCardJson adaptiveCard)
        =>
        new()
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = adaptiveCard
        };

    private static void SetReplyFields(this Activity originalActivity, IMessageActivity reply)
    {
        var tempReply = originalActivity.CreateReply(string.Empty);

        reply.ChannelId = tempReply.ChannelId;
        reply.Timestamp = tempReply.Timestamp;
        reply.From = tempReply.From;
        reply.Conversation = tempReply.Conversation;
        reply.Recipient = tempReply.Recipient;
        reply.Id = tempReply.Id;
        reply.ReplyToId = tempReply.ReplyToId;

        if (reply.Type is null)
        {
            reply.Type = ActivityTypes.Message;
        }
    }
}