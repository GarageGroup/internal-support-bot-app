using GarageGroup.Infra;
using System;
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
                    CaseTypeTemplate = new(
                        role: "some role",
                        contentTemplate: "some case type template")
                },
                new("some message"),
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
                                Content = "some content template some message"
                            },
                            new()
                            {
                                Role = "some role",
                                Content = "some case type template"
                            },
                        ]
                    })
                }
            }
        };
}
