using System;
using System.Collections.Generic;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static IEnumerable<object[]> InputTestData
        =>
        new[]
        {
            new object[]
            {
                new SupportGptApiOption(
                    apiKey: "Some Key",
                    incidentComplete: new("some-model")
                    {
                        ChatMessages = default
                    }),
                new IncidentCompleteIn("Some customer request"),
                new StubGptJsonIn
                {
                    Messages = Array.Empty<StubMessageJson>(),
                    Model = "some-model",
                    Top = 1
                }
                .ToJson(),
                "Bearer Some Key"
            },
            new object[]
            {
                new SupportGptApiOption(
                    apiKey: "Some API key",
                    incidentComplete: new("some-gpt-model")
                    {
                        ChatMessages = new ChatMessageOption("some-role", "Some message: {0}").AsFlatArray(),
                        MaxTokens = 150,
                        Temperature = 0.7m
                    }),
                new IncidentCompleteIn("Some request message"),
                new StubGptJsonIn
                {
                    Messages = new StubMessageJson[]
                    {
                        new()
                        {
                            Role = "some-role",
                            Content = "Some message: Some request message"
                        }
                    },
                    Model = "some-gpt-model",
                    MaxTokens = 150,
                    Top = 1,
                    Temperature = 0.7m
                }
                .ToJson(),
                "Bearer Some API key"
            },
            new object[]
            {
                new SupportGptApiOption(
                    apiKey: "Some Key",
                    incidentComplete: new("some-model")
                    {
                        ChatMessages = new ChatMessageOption[]
                        {
                            new("role-first", "Some first message"),
                            new("role-second", "{0}: Some second message")
                        }
                    }),
                new IncidentCompleteIn("\nSome customer request\t"),
                new StubGptJsonIn
                {
                    Messages = new StubMessageJson[]
                    {
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
                    },
                    Model = "some-model",
                    Top = 1
                }
                .ToJson(),
                "Bearer Some Key"
            }
        };
}