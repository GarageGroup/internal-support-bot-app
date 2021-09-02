using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra
{
    internal static class BotMiddleware
    {
        private static readonly object lockObject = new();

        private static volatile AdapterWithErrorHandler? adapter;

        public static IApplicationBuilder UseBot(this IApplicationBuilder builder, Func<IServiceProvider, IBot> botResolver)
            =>
            builder.Map("/api/messages", app => app.Use(_ => ctx => InvokeBotAsync(ctx, botResolver)));

        private static Task InvokeBotAsync(HttpContext context, Func<IServiceProvider, IBot> botResolver)
            =>
            context.RequestServices.GetAdapterWithErrorHandler()
            .ProcessAsync(
                httpRequest: context.Request,
                httpResponse: context.Response,
                bot: botResolver.Invoke(context.RequestServices),
                cancellationToken: context.RequestAborted);

        private static AdapterWithErrorHandler GetAdapterWithErrorHandler(this IServiceProvider serviceProvider)
        {
            if (adapter is not null)
            {
                return adapter;
            }

            lock (lockObject)
            {
                if (adapter is not null)
                {
                    return adapter;
                }

                adapter = new(
                    configuration: serviceProvider.GetRequiredService<IConfiguration>(),
                    logger: serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<AdapterWithErrorHandler>());
            }

            return adapter;
        }
    }
}