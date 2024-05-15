using System;
using System.Collections.Generic;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static IBotBuilder UseBotInfoFlow(this IBotBuilder botBuilder)
        =>
        botBuilder.UseBotInfo(BotInfoCommand, GetBotInfoData);

    private static BotInfoData GetBotInfoData(IBotContext botContext)
        =>
        new() 
        { 
            Values = botContext.ServiceProvider.GetRequiredService<IConfiguration>().GetRequiredSection("Info").GetBotInfoData() 
        };

    private static FlatArray<KeyValuePair<string, string?>> GetBotInfoData(this IConfigurationSection section)
        =>
        [
            new("Название", section["ApiName"]),
            new("Описание", section["Description"]),
            new("Версия сборки", section["ApiVersion"]),
            new("Время сборки", section.GetValue<DateTimeOffset?>("BuildDateTime").ToRussianStandardTimeZoneString())
        ];
}