using GarageGroup.Infra;
using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<SupportGptApiOption, IncidentCompleteIn, HttpSendIn> InputTestData
        =>
        new()
        {
            {
                new(
                    chatMessages: new FlatArray<ChatMessageOption>(new ChatMessageOption("some role", "some content template {0}")))
                {
                    MaxTokens = 30,
                    Temperature = 0
                },
                new("some message"),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn()
                    {
                        MaxTokens = 30,
                        Temperature = 0,
                        Top = 1,
                        Messages = new FlatArray<ChatMessageJson>(new ChatMessageJson()
                        {
                            Role = "some role",
                            Content = "some content template some message"
                        })
                    })
                }
            },
            {
                new(
                    chatMessages: new FlatArray<ChatMessageOption>(new ChatMessageOption("some role", "some content template {0}")))
                {
                    MaxTokens = 100,
                    Temperature = 0.2m
                },
                new("  some message trim   "),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn()
                    {
                        MaxTokens = 100,
                        Temperature = 0.2m,
                        Top = 1,
                        Messages = new FlatArray<ChatMessageJson>(new ChatMessageJson()
                        {
                            Role = "some role",
                            Content = "some content template some message trim"
                        })
                    })
                }
            }
        };
}