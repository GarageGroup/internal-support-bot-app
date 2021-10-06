using System;

namespace GGroupp.Internal.Support.Bot;

public sealed record ADUserApiClientConfiguration : IADUserApiClientConfiguration
{
    public string? GraphApiBaseAddressUrl { get; init; }

    Uri IADUserApiClientConfiguration.GraphApiBaseAddress => new(GraphApiBaseAddressUrl.OrEmpty());
}
