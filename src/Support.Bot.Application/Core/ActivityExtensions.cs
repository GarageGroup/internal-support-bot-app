using System;
using System.Collections.Generic;
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

    public static IMessageActivity CreateReplyWithAttachment(this Activity originalActivity, Attachment attachment)
        =>
        InnerCreateReplyWithAttachment(
            originalActivity ?? throw new ArgumentNullException(nameof(originalActivity)),
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

    private static IMessageActivity InnerCreateReplyWithAttachment(this Activity originalActivity, Attachment attachment)
    {
        var reply = Activity.CreateMessageActivity();

        if (reply.Attachments is null)
        {
            reply.Attachments = new List<Attachment>();
        }
        reply.Attachments.Add(attachment);
        return reply;
        /*var reply = new Activity
        {
            Attachments = new[] { attachment }
        };

        originalActivity.SetReplyFields(reply);
        return reply;*/
    }

    /*private static void SetReplyFields(this Activity originalActivity, IMessageActivity reply)
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
    }*/
}