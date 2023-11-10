using System.Collections.Generic;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static IEnumerable<object[]> InputAzureTestData
        =>
        new object[][]
        {
            [
                new SupportGptApiOption(
                    apiKey: "SomeKey",
                    azureGpt: new(
                        resourceName: "some-resource-name",
                        deploymentId: "some-deployment-id",
                        apiVersion: "some-api-version"),
                    incidentComplete: new(default)),
                new IncidentCompleteIn("Some customer request"),
                "https://some-resource-name.openai.azure.com/openai/deployments/some-deployment-id/chat/completions?api-version=some-api-version",
                new StubGptJsonIn
                {
                    Messages = [],
                    Top = 1
                }
                .ToJson(),
                new KeyValuePair<string, string>("api-key", "SomeKey")
            ],
            [
                new SupportGptApiOption(
                    apiKey: "Some API key",
                    azureGpt: new(
                        resourceName: "some-resource-name",
                        deploymentId: "some-deployment-id",
                        apiVersion: "2023-07-01-preview"),
                    incidentComplete: new(
                        chatMessages: new ChatMessageOption[]
                        {
                            new("some-role", "Some message: {0}")
                        })
                    {
                        MaxTokens = 150,
                        Temperature = 0.7m
                    }),
                new IncidentCompleteIn("Some request message"),
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
                new KeyValuePair<string, string>("api-key", "Some API key")
            ],
            [
                new SupportGptApiOption(
                    apiKey: "Some API Key",
                    azureGpt: new(
                        resourceName: "someResource",
                        deploymentId: "someDeployment",
                        apiVersion: "2023-07-01-preview"),
                    incidentComplete: new(
                        chatMessages: new ChatMessageOption[]
                        {
                            new("role-first", "Some first message"),
                            new("role-second", "{0}: Some second message")
                        })),
                new IncidentCompleteIn("\nSome customer request\t"),
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
                new KeyValuePair<string, string>("api-key", "Some API Key")
            ]
        };
}