using System.Collections.Generic;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<SupportGptApiOption, IncidentCompleteIn, string, string, KeyValuePair<string, string>> InputOpenAiTestData
        =>
        new()
        {
            {
                new(
                    apiKey: "Some Key",
                    model: "some-model",
                    incidentComplete: new(default)),
                new("Some customer request"),
                "https://api.openai.com/v1/chat/completions",
                new StubGptJsonIn
                {
                    Messages = [],
                    Model = "some-model",
                    Top = 1
                }
                .ToJson(),
                new("Authorization", "Bearer Some Key")
            },
            {
                new(
                    apiKey: "Some API key",
                    model: "some-gpt-model",
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
                new("Authorization", "Bearer Some API key")
            },
            {
                new(
                    apiKey: "SomeKey",
                    model: "some-model",
                    incidentComplete: new(
                        chatMessages:
                        [
                            new("role-first", "Some first message"),
                            new("role-second", "{0}: Some second message")
                        ])),
                new("\nSome customer request\t"),
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
                new("Authorization", "Bearer SomeKey")
            }
        };
}