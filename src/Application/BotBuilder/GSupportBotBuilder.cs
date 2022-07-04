using System;
using System.Net.Http;
using GGroupp.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

internal static partial class GSupportBotBuilder
{
    private static Dependency<HttpMessageHandler> CreateStandardHttpHandlerDependency(string loggerCategoryName)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(
            sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger(loggerCategoryName));

    private static IConfigurationSection GetRequiredSection(this IServiceProvider serviceProvider, string sectionName)
        =>
        serviceProvider.GetRequiredService<IConfiguration>().GetRequiredSection(sectionName);
}