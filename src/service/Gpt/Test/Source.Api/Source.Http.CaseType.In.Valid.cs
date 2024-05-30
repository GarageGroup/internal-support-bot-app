using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<SupportGptApiOption, IncidentCompleteIn, HttpSendIn> InputCaseTypeTestData
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
                    CaseTypeTemplate = new(
                        role: "some role",
                        contentTemplate: "some case type template")
                },
                new("some message", default),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn
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
                            },
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(text: "some case type template")
                                ]
                            },
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
                    MaxTokens = 30,
                    Temperature = 0,
                    IsImageProcessing = true,
                    CaseTypeTemplate = new(
                        role: "some role",
                        contentTemplate: "some case type template")
                },
                new("some message", new("some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn
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
                                    new(image: new("some image")),
                                    new(text: "some content template some message")
                                ]
                            },
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(text: "some case type template")
                                ]
                            },
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
                    MaxTokens = 30,
                    Temperature = 0,
                    IsImageProcessing = true,
                    CaseTypeTemplate = new(
                        role: "some role",
                        contentTemplate: "some case type template")
                },
                new("some message", new("first some image", "second some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn
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
                                    new(image: new("first some image")),
                                    new(image: new("second some image")),
                                    new(text: "some content template some message")
                                ]
                            },
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(text: "some case type template")
                                ]
                            },
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
                    MaxTokens = 30,
                    Temperature = 0,
                    IsImageProcessing = true,
                    CaseTypeTemplate = new(
                        role: "some role",
                        contentTemplate: "some case type template")
                },
                new(null, new("some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn
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
                                    new(image: new("some image"))
                                ]
                            },
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(text: "some case type template")
                                ]
                            },
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
                    MaxTokens = 30,
                    Temperature = 0,
                    IsImageProcessing = false,
                    CaseTypeTemplate = new(
                        role: "some role",
                        contentTemplate: "some case type template")
                },
                new("some message", new("some image")),
                new(
                    method: HttpVerb.Post,
                    requestUri: string.Empty)
                {
                    Body = HttpBody.SerializeAsJson(new ChatGptJsonIn
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
                            },
                            new()
                            {
                                Role = "some role",
                                Content =
                                [
                                    new(text: "some case type template")
                                ]
                            },
                        ]
                    })
                }
            }
        };
}
