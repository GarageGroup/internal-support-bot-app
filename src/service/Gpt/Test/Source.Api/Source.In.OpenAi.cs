using System.Collections.Generic;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static IEnumerable<object[]> InputOpenAiTestData
        =>
        new object[][]
        {
            [
                new SupportGptApiOption(
                    apiKey: "Some Key",
                    model: "some-model",
                    incidentComplete: new(default)),
                new IncidentCompleteIn("Some customer request"),
                "https://api.openai.com/v1/chat/completions",
                new StubGptJsonIn
                {
                    Messages = [],
                    Model = "some-model",
                    Top = 1
                }
                .ToJson(),
                new KeyValuePair<string, string>("Authorization", "Bearer Some Key")
            ],
            [
                new SupportGptApiOption(
                    apiKey: "Some API key",
                    model: "some-gpt-model",
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
                "https://api.openai.com/v1/chat/completions",
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
                    Model = "some-gpt-model",
                    MaxTokens = 150,
                    Top = 1,
                    Temperature = 0.7m
                }
                .ToJson(),
                new KeyValuePair<string, string>("Authorization", "Bearer Some API key")
            ],
            [
                new SupportGptApiOption(
                    apiKey: "SomeKey",
                    model: "some-model",
                    incidentComplete: new(
                        chatMessages: new ChatMessageOption[]
                        {
                            new("role-first", "Some first message"),
                            new("role-second", "{0}: Some second message")
                        })),
                new IncidentCompleteIn("\nSome customer request\t"),
                "https://api.openai.com/v1/chat/completions",
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
                    Model = "some-model",
                    Top = 1
                }
                .ToJson(),
                new KeyValuePair<string, string>("Authorization", "Bearer SomeKey")
            ]
        };
}