using System;
using GGroupp.Infra;

namespace GGroupp.Internal.Support.Bot;

internal sealed record DataverseApiClientConfiguration : IDataverseApiClientConfiguration
{
    public string? DataverseApiServiceUrl { get; init; }

    public string? DataverseApiVersion { get; init; }

    public string? DataverseApiAuthTenantId { get; init; }

    public string? DataverseApiAuthClientId { get; init; }

    public string? DataverseApiAuthClientSecret { get; init; }

    string IDataverseApiClientConfiguration.ServiceUrl => DataverseApiServiceUrl.OrEmpty();

    string IDataverseApiClientConfiguration.ApiVersion => DataverseApiVersion.OrEmpty();

    string IDataverseApiClientConfiguration.AuthTenantId => DataverseApiAuthTenantId.OrEmpty();

    string IDataverseApiClientConfiguration.AuthClientId => DataverseApiAuthClientId.OrEmpty();

    string IDataverseApiClientConfiguration.AuthClientSecret => DataverseApiAuthClientSecret.OrEmpty();
}
