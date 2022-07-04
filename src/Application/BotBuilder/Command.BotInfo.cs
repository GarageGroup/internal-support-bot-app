using System;
using System.Collections.Generic;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;

namespace GGroupp.Internal.Support;

partial class GSupportBotBuilder
{
    internal static IBotBuilder UseGSupportBotInfo(this IBotBuilder botBuilder, string commandName)
        =>
        botBuilder.UseBotInfo(commandName, GetBotInfoData);

    private static BotInfoData GetBotInfoData(IBotContext botContext)
        =>
        new(botContext.ServiceProvider.GetRequiredSection("BotInfo").GetBotInfoData());

    private static IReadOnlyCollection<KeyValuePair<string, string?>> GetBotInfoData(this IConfigurationSection section)
        =>
        new KeyValuePair<string, string?>[]
        {
            new("Название", section["Name"]),
            new("Описание", section["Description"]),
            new("Версия сборки", section["BuildVersion"]),
            new("Время сборки", section.GetValue<DateTimeOffset?>("BotBuildDateTime").ToRussianStandardTimeZoneString())
        };
}