using System;

namespace GarageGroup.Internal.Support;

public sealed record class SupportAzureGptApiOption
{
    public SupportAzureGptApiOption(string resourceName, string deploymentId, string apiVersion)
    {
        ResourceName = resourceName.OrEmpty();
        DeploymentId = deploymentId.OrEmpty();
        ApiVersion = apiVersion.OrEmpty();
    }

    public string ResourceName { get; }

    public string DeploymentId { get; }

    public string ApiVersion { get; }
}