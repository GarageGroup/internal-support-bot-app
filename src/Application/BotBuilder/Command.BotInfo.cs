using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GGroupp.Internal.Support;

partial class GSupportBotBuilder
{
    internal static IBotBuilder UseGSupportBotInfo(this IBotBuilder botBuilder, string commandName)
        =>
        botBuilder.UseBotInfo(commandName, GetBotInfoData);

    private static readonly Lazy<TimeZoneInfo> lazyRussianStandardTimeZone
        =
        new(GetRussianStandardTimeZone, LazyThreadSafetyMode.ExecutionAndPublication);

    private static BotInfoData GetBotInfoData(IBotContext botContext)
        =>
        new(botContext.ServiceProvider.GetRequiredService<IConfiguration>().GetBotInfoData());

    private static IReadOnlyCollection<KeyValuePair<string, string?>> GetBotInfoData(this IConfiguration configuration)
        =>
        new KeyValuePair<string, string?>[]
        {
            new("Название", configuration.GetValue<string>("BotName")),
            new("Описание", configuration.GetValue<string>("BotDescription")),
            new("Версия сборки", configuration.GetValue<string>("BotBuildVersion")),
            new("Время сборки", configuration.GetValue<DateTimeOffset?>("BotBuildDateTime").ToRussianStandardTimeZoneString())
        };

    private static string? ToRussianStandardTimeZoneString(this DateTimeOffset? dateTime)
    {
        if (dateTime is null)
        {
            return default;
        }

        var russianStandardTime = TimeZoneInfo.ConvertTime(dateTime.Value, lazyRussianStandardTimeZone.Value);
        return russianStandardTime.ToString("dd.MM.yyyy HH:mm:ss ('GMT'z)", CultureInfo.InvariantCulture);
    }

    private static TimeZoneInfo GetRussianStandardTimeZone()
        =>
        TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
}