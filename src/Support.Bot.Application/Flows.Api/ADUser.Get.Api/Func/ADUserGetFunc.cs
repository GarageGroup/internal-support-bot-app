using System;
using System.Net.Http;

namespace GGroupp.Internal.Support.Bot
{
    using IADUserGetFunc = IAsyncValueFunc<ADUserGetIn, Result<ADUserGetOut, Failure<Unit>>>;

    internal sealed partial class ADUserGetFunc : IADUserGetFunc
    {
        public static ADUserGetFunc Create(
            HttpMessageHandler httpMessageHandler,
            IADUserApiClientConfiguration apiClientConfiguration)
            =>
            new(
                httpMessageHandler ?? throw new ArgumentNullException(nameof(httpMessageHandler)),
                apiClientConfiguration ?? throw new ArgumentNullException(nameof(apiClientConfiguration)));

        private readonly HttpMessageHandler httpMessageHandler;

        private readonly IADUserApiClientConfiguration apiClientConfiguration;

        private ADUserGetFunc(
            HttpMessageHandler httpMessageHandler,
            IADUserApiClientConfiguration apiClientConfiguration)
        {
            this.httpMessageHandler = httpMessageHandler;
            this.apiClientConfiguration = apiClientConfiguration;
        }
    }
}