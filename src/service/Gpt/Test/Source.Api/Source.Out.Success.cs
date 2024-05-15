using GarageGroup.Infra;
using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<HttpSendOut, Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> OutputSuccessTestData
        =>
        new()
        {
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
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
                    }),
                },
                new IncidentCompleteOut()
                {
                    Title = "Some response \"message\""
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
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
                    }),
                },
                new IncidentCompleteOut()
                {
                    Title = null
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
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
                    }),
                },
                new IncidentCompleteOut()
                {
                    Title = "Some response message"
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
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
                    }),
                },
                new IncidentCompleteOut()
                {
                    Title = "Some response \"message"
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
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
                    }),
                },
                new IncidentCompleteOut()
                {
                    Title = null
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
                    {
                        Choices = default
                    }),
                },
                new Failure<IncidentCompleteFailureCode>(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"GPT result choices are absent. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut()
                    {
                        Choices = default
                    })}'")
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut()
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "Some response \"message\""
                                },
                                FinishReason = "not stop"
                            }
                        ]
                    }),
                },
                new Failure<IncidentCompleteFailureCode>(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"An unexpected GPT finish reason: 'not stop'. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut()
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "Some response \"message\""
                                },
                                FinishReason = "not stop"
                            }
                        ]
                    })}'")
            }
        };
}