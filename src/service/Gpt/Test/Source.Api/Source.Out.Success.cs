using System;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<HttpSendOut, HttpSendOut, Result<IncidentCompleteOut, Failure<IncidentCompleteFailureCode>>> OutputSuccessTestData
        =>
        new()
        {
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "2"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                new IncidentCompleteOut
                {
                    Title = "Some response \"message\"",
                    CaseTypeCode = IncidentCaseTypeCode.Request
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                        ]
                    }),
                },
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "1"
                                },
                                FinishReason = "stop"
                            },
                        ]
                    }),
                },
                new IncidentCompleteOut
                {
                    Title = null,
                    CaseTypeCode = IncidentCaseTypeCode.Problem
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "0"
                                },
                                FinishReason = "stop"
                            },
                        ]
                    }),
                },
                new IncidentCompleteOut
                {
                    Title = "Some response message",
                    CaseTypeCode = IncidentCaseTypeCode.Question
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "1"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                new IncidentCompleteOut
                {
                    Title = "Some response \"message",
                    CaseTypeCode = IncidentCaseTypeCode.Problem
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "1"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                new IncidentCompleteOut
                {
                    Title = null,
                    CaseTypeCode = IncidentCaseTypeCode.Problem
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices = default
                    }),
                },
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "1"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                Failure.Create(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"GPT result choices are absent. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices = default
                    })}'")
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "1"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                Failure.Create(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"An unexpected GPT finish reason: 'not stop'. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut
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
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices = default
                    }),
                },
                Failure.Create(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"GPT result choices are absent. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices = default
                    })}'")
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "1"
                                },
                                FinishReason = "not stop"
                            }
                        ]
                    }),
                },
                Failure.Create(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"An unexpected GPT finish reason: 'not stop'. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "1"
                                },
                                FinishReason = "not stop"
                            }
                        ]
                    })}'")
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "-1"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                Failure.Create(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"Incorrect response from GPT. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "-1"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    })}'")
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "4"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                Failure.Create(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"Incorrect response from GPT. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "4"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    })}'")
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
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
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "Some content"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    }),
                },
                Failure.Create(
                    failureCode: IncidentCompleteFailureCode.Unknown,
                    failureMessage: $"Incorrect response from GPT. Body: '{HttpBody.SerializeAsJson(new StubGptJsonOut
                    {
                        Choices =
                        [
                            new()
                            {
                                Message = new()
                                {
                                    Content = "Some content"
                                },
                                FinishReason = "stop"
                            }
                        ]
                    })}'")
            }
        };
}