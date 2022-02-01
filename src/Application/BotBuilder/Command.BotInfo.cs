using System;
using System.Collections.Generic;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GGroupp.Internal.Support;

partial class GSupportBotBuilder
{
    internal static IBotBuilder UseGSupportBotInfo(this IBotBuilder botBuilder, string commandName)
        =>
        botBuilder.UseBotInfo(commandName, GetBotInfoData);

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
}