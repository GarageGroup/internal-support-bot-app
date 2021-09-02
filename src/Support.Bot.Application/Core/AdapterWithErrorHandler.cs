using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace GGroupp.Infra
{
    internal sealed class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        static AdapterWithErrorHandler()
            =>
            HttpHelper.BotMessageSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                await turnContext.SendActivityAsync("Что-то пошло не так...");
            };
        }
    }
}
