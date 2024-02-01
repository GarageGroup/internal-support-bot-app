using System.Collections.Generic;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<SupportGptApiOption, IncidentCompleteIn, string, string, KeyValuePair<string, string>> InputAzureTestData
        =>
        new()
        {
            {
                new(
                    apiKey: "SomeKey",
                    azureGpt: new(
                        resourceName: "some-resource-name",
                        deploymentId: "some-deployment-id",
                        apiVersion: "some-api-version"),
                    incidentComplete: new(default)),
                new("Some customer request"),
                "https://some-resource-name.openai.azure.com/openai/deployments/some-deployment-id/chat/completions?api-version=some-api-version",
                new StubGptJsonIn
                {
                    Messages = [],
                    Top = 1
                }
                .ToJson(),
                new("api-key", "SomeKey")
            },
            {
                new(
                    apiKey: "Some API key",
                    azureGpt: new(
                        resourceName: "some-resource-name",
                        deploymentId: "some-deployment-id",
                        apiVersion: "2023-07-01-preview"),
                    incidentComplete: new(
                        chatMessages:
                        [
                            new("some-role", "Some message: {0}")
                        ])
                    {
                        MaxTokens = 150,
                        Temperature = 0.7m
                    }),
                new("Some request message"),
                "https://some-resource-name.openai.azure.com/openai/deployments/some-deployment-id/chat/completions?api-version=2023-07-01-preview",
                new StubGptJsonIn
                {
                    Messages =
                    [
                        new()
                        {
                            Role = "some-role",
                            Content = "Some message: Some request message"
                        }
                    ],
                    MaxTokens = 150,
                    Top = 1,
                    Temperature = 0.7m
                }
                .ToJson(),
                new("api-key", "Some API key")
            },
            {
                new(
                    apiKey: "Some API Key",
                    azureGpt: new(
                        resourceName: "someResource",
                        deploymentId: "someDeployment",
                        apiVersion: "2023-07-01-preview"),
                    incidentComplete: new(
                        chatMessages:
                        [
                            new("role-first", "Some first message"),
                            new("role-second", "{0}: Some second message")
                        ])),
                new("\nSome customer request\t"),
                "https://someResource.openai.azure.com/openai/deployments/someDeployment/chat/completions?api-version=2023-07-01-preview",
                new StubGptJsonIn
                {
                    Messages =
                    [
                        new()
                        {
                            Role = "role-first",
                            Content = "Some first message"
                        },
                        new()
                        {
                            Role = "role-second",
                            Content = "Some customer request: Some second message"
                        }
                    ],
                    Top = 1
                }
                .ToJson(),
                new("api-key", "Some API Key")
            }
        };
}