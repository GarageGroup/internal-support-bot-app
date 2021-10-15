using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Bot;

internal static partial class BotServiceProvider
{
    public static ILogger GetLogger(this IServiceProvider serviceProvider, [AllowNull] string loggerCategoryName)
        =>
        serviceProvider.Pipe(GetLoggerFactory).CreateLogger(loggerCategoryName.OrEmpty());

    public static ILoggerFactory GetLoggerFactory(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredService<ILoggerFactory>();

    public static TConfiguration GetConfiguration<TConfiguration>(IServiceProvider serviceProvider)
        where TConfiguration : class, new()
        =>
        serviceProvider.GetRequiredService<IConfiguration>().Get<TConfiguration>() ?? new();
}
