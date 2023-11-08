using System.Collections.Generic;
using System.Net;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static IEnumerable<object[]> OutputSuccessTestData
        =>
        new[]
        {
            new object[]
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices = new StubChoiceJson[]
                    {
                        new()
                        {
                            Message = new()
                            {
                                Content = "Some response \"message\""
                            },
                            FinishReason = "stop"
                        }
                    }
                }
                .ToJson(),
                new IncidentCompleteOut
                {
                    Title = "Some response \"message\""
                }
            },
            new object[]
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices = new StubChoiceJson[]
                    {
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
                    }
                }
                .ToJson(),
                new IncidentCompleteOut
                {
                    Title = null
                }
            },
            new object[]
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices = new StubChoiceJson[]
                    {
                        new()
                        {
                            Message = new()
                            {
                                Content = "\"Some response message.\""
                            },
                            FinishReason = "stop"
                        }
                    }
                }
                .ToJson(),
                new IncidentCompleteOut
                {
                    Title = "Some response message"
                }
            },
            new object[]
            {
                HttpStatusCode.Accepted,
                new StubGptJsonOut
                {
                    Choices = new StubChoiceJson[]
                    {
                        new()
                        {
                            Message = new()
                            {
                                Content = "\n\"Some response \"message\"\t."
                            },
                            FinishReason = "stop"
                        }
                    }
                }
                .ToJson(),
                new IncidentCompleteOut
                {
                    Title = "Some response \"message"
                }
            },
            new object[]
            {
                HttpStatusCode.OK,
                new StubGptJsonOut
                {
                    Choices = new StubChoiceJson[]
                    {
                        new()
                        {
                            Message = new()
                            {
                                Content = "\"\""
                            },
                            FinishReason = "stop"
                        }
                    }
                }
                .ToJson(),
                new IncidentCompleteOut
                {
                    Title = null
                }
            }
        };
}