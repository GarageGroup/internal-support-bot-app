using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<SupportGptApiOption, IncidentCompleteIn, HttpSendIn> InputTitleTestData
        =>
        new()
        {
            {
                new(
                    chatMessages: 
                    [
                        new(
                        role: "some role",
                        contentTemplate: "some content template {0}")
                    ])
                {
                    MaxTokens = 30,
                    Temperature = 0,
                    IsImageProcessing = true,
                },
                new("some message", default),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn()
                    {
                        MaxTokens = 30,
                        Temperature = 0,
                        Top = 1,
                        Messages = 
                        [
                            new()
                            {
                                Role = "some role",
                                Content =
                                [ 
                                    new(text: "some content template some message")
                                ]
                            }
                        ]
                    })
                }
            },
            {
                new(
                    chatMessages:
                    [
                        new(
                        role: "some role",
                        contentTemplate: "some content template {0}")
                    ])
                {
                    MaxTokens = 100,
                    Temperature = 0.2m,
                    IsImageProcessing = true,
                },
                new("  some message trim   ", new("some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn()
                    {
                        MaxTokens = 100,
                        Temperature = 0.2m,
                        Top = 1,
                        Messages = 
                        [
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(image: new("some image")),
                                    new(text: "some content template some message trim")
                                ]
                            }
                        ]
                    })
                }
            },
            {
                new(
                    chatMessages:
                    [
                        new(
                        role: "some role",
                        contentTemplate: "some content template {0}")
                    ])
                {
                    MaxTokens = 100,
                    Temperature = 0.2m,
                    IsImageProcessing = true,
                },
                new("  some message trim   ", new("first some image", "second some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn()
                    {
                        MaxTokens = 100,
                        Temperature = 0.2m,
                        Top = 1,
                        Messages =
                        [
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(image: new("first some image")),
                                    new(image: new("second some image")),
                                    new(text: "some content template some message trim")
                                ]
                            }
                        ]
                    })
                }
            },
            {
                new(
                    chatMessages:
                    [
                        new(
                        role: "some role",
                        contentTemplate: "some content template {0}")
                    ])
                {
                    MaxTokens = 100,
                    Temperature = 0.2m,
                    IsImageProcessing = true,
                },
                new(null, new("some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn()
                    {
                        MaxTokens = 100,
                        Temperature = 0.2m,
                        Top = 1,
                        Messages =
                        [
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(image: new("some image"))
                                ]
                            }
                        ]
                    })
                }
            },
            {
                new(
                    chatMessages:
                    [
                        new(
                        role: "some role",
                        contentTemplate: "some content template {0}")
                    ])
                {
                    MaxTokens = 100,
                    Temperature = 0.2m,
                    IsImageProcessing = false,
                },
                new("some message", new("some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn()
                    {
                        MaxTokens = 100,
                        Temperature = 0.2m,
                        Top = 1,
                        Messages =
                        [
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(text: "some content template some message")
                                ]
                            }
                        ]
                    })
                }
            },
        };
}