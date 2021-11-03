using System;

namespace GGroupp.Internal.Support.Bot;

internal sealed record ADUserApiClientConfiguration : IADUserApiClientConfiguration
{
    public string? GraphApiBaseAddressUrl { get; init; }

    Uri IADUserApiClientConfiguration.GraphApiBaseAddress => new(GraphApiBaseAddressUrl.OrEmpty());
}
