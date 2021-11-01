using System;
using GGroupp.Infra;

namespace GGroupp.Internal.Support.Bot;

internal sealed record DataverseApiClientConfiguration : IDataverseApiClientConfiguration, IIncidentCreateFlowConfiguration
{
    public string? DataverseApiServiceUrl { get; init; }

    public string? DataverseApiAuthTenantId { get; init; }

    public string? DataverseApiAuthClientId { get; init; }

    public string? DataverseApiAuthClientSecret { get; init; }

    public string? IncidentCardRelativeUrlTemplate { get; init; }

    public int? CaseOriginCode { get; init; }

    string IDataverseApiClientConfiguration.ServiceUrl => DataverseApiServiceUrl.OrEmpty();

    string IDataverseApiClientConfiguration.AuthTenantId => DataverseApiAuthTenantId.OrEmpty();

    string IDataverseApiClientConfiguration.AuthClientId => DataverseApiAuthClientId.OrEmpty();

    string IDataverseApiClientConfiguration.AuthClientSecret => DataverseApiAuthClientSecret.OrEmpty();

    string IIncidentCreateFlowConfiguration.IncidentCardUrlTemplate
    {
        get
        {
            var baseUri = new Uri(DataverseApiServiceUrl.OrEmpty());

            return new Uri(baseUri, IncidentCardRelativeUrlTemplate).AbsoluteUri
                .Replace("%7B", "{", StringComparison.InvariantCultureIgnoreCase)
                .Replace("%7D", "}", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
