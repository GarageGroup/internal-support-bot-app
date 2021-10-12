using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace GGroupp.Infra;

public static class ActivityExtensions
{
    public static IMessageActivity ToActivity(this Attachment attachment)
        =>
        MessageFactory.Attachment(
            attachment ?? throw new ArgumentNullException(nameof(attachment)));

    public static bool IsCardSupported(this Activity activity)
        =>
        InnerIsCardSupported(
            activity ?? throw new ArgumentNullException(nameof(activity)));

    public static Result<Guid, Unit> GetGuidValueOrAbsent(this Activity activity)
        =>
        InnerGetGuidValueOrAbsent(
            activity ?? throw new ArgumentNullException(nameof(activity)));

    private static bool InnerIsCardSupported(this Activity activity)
        =>
        activity.ChannelId switch
        {
            Channels.Msteams => true,
            Channels.Webchat => true,
            Channels.Emulator => true,
            _ => false
        };

    private static Result<Guid, Unit> InnerGetGuidValueOrAbsent(this Activity activity)
        =>
        activity.Type != ActivityTypes.Message
            ? Result.Absent<Guid>()
            : activity.ParseGuidValueOrAbsent().ToResult();

    private static Optional<Guid> ParseGuidValueOrAbsent(this Activity activity)
        =>
        Pipeline.Pipe(
            activity.Text.ParseGuidOrAbsent())
        .Or(
            activity.Value.ToStringOrEmpty().ParseGuidOrAbsent);

    private static Optional<Guid> ParseGuidOrAbsent(this string? value)
        =>
        string.IsNullOrEmpty(value) is false && Guid.TryParse(value, out var guid) ? guid : default(Optional<Guid>);
}