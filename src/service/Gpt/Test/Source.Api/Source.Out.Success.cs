using System.Net;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<HttpStatusCode, string, IncidentCompleteOut> OutputSuccessTestData
        =>
        new()
        {
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices =
                    [
                        new()
                        {
                            Message = new()
                            {
                                Content = "Some response \"message\""
                            },
                            FinishReason = "stop"
                        }
                    ]
                }
                .ToJson(),
                new()
                {
                    Title = "Some response \"message\""
                }
            },
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices =
                    [
                        new()
                        {
                            Message = new()
                            {
                                Content = string.Empty
                            },
                            FinishReason = "stop"
                        },
                        new()
                        {
                            Message = new()
                            {
                                Content = "Some message"
                            },
                            FinishReason = "stop"
                        }
                    ]
                }
                .ToJson(),
                new()
                {
                    Title = null
                }
            },
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices =
                    [
                        new()
                        {
                            Message = new()
                            {
                                Content = "\"Some response message.\""
                            },
                            FinishReason = "stop"
                        }
                    ]
                }
                .ToJson(),
                new()
                {
                    Title = "Some response message"
                }
            },
            {
                HttpStatusCode.Accepted,
                new StubGptJsonOut
                {
                    Choices =
                    [
                        new()
                        {
                            Message = new()
                            {
                                Content = "\n\"Some response \"message\"\t."
                            },
                            FinishReason = "stop"
                        }
                    ]
                }
                .ToJson(),
                new()
                {
                    Title = "Some response \"message"
                }
            },
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices =
                    [
                        new()
                        {
                            Message = new()
                            {
                                Content = "\"\""
                            },
                            FinishReason = "stop"
                        }
                    ]
                }
                .ToJson(),
                new()
                {
                    Title = null
                }
            }
        };
}