using System;
using System.Collections.Generic;
using System.Net;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static IEnumerable<object?[]> OutputFailureTestData
    {
        get
        {
            var firstJson = new StubGptJsonOut
            {
                Choices = new StubChoiceJson[]
                {
                    new()
                    {
                        Message = new()
                        {
                            Content = "Some first message"
                        },
                        FinishReason = "failed"
                    },
                    new()
                    {
                        Message = new()
                        {
                            Content = "Some second message"
                        },
                        FinishReason = "stop"
                    }
                }
            }
            .ToJson();

            yield return new object?[]
            {
                HttpStatusCode.OK,
                firstJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"An unexpected GPT finish reason: failed. Body: {firstJson}")
            };

            var secondJson = new StubGptJsonOut
            {
                Choices = Array.Empty<StubChoiceJson>()
            }
            .ToJson();

            yield return new object?[]
            {
                HttpStatusCode.OK,
                secondJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"GPT result choices are absent. Body: {secondJson}")
            };

            var thirdJson = new StubGptJsonOut
            {
                Choices = null
            }
            .ToJson();

            yield return new object?[]
            {
                HttpStatusCode.OK,
                thirdJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"GPT result choices are absent. Body: {thirdJson}")
            };

            var fourthJson = new StubFailureJson
            {
                Error = new()
                {
                    Message = "Some error message"
                }
            }
            .ToJson();

            yield return new object?[]
            {
                HttpStatusCode.BadRequest,
                fourthJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"An unexpected http status code: BadRequest. Body: {fourthJson}")
            };

            yield return new object?[]
            {
                HttpStatusCode.Unauthorized,
                "Some error content",
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    "An unexpected http status code: Unauthorized. Body: Some error content")
            };

            yield return new object?[]
            {
                HttpStatusCode.NotFound,
                null,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    "An unexpected http status code: NotFound. Body: ")
            };

            yield return new object?[]
            {
                HttpStatusCode.TooManyRequests,
                new StubFailureJson
                {
                    Error = new()
                    {
                        Message = "Some error message"
                    }
                }
                .ToJson(),
                Failure.Create(
                    IncidentCompleteFailureCode.TooManyRequests,
                    "Some error message")
            };

            var fifthJson = new StubFailureJson
            {
                Error = new()
                {
                    Message = null
                }
            }
            .ToJson();

            yield return new object?[]
            {
                HttpStatusCode.TooManyRequests,
                fifthJson,
                Failure.Create(
                    IncidentCompleteFailureCode.TooManyRequests,
                    fifthJson)
            };

            yield return new object?[]
            {
                HttpStatusCode.TooManyRequests,
                null,
                Failure.Create(
                    IncidentCompleteFailureCode.TooManyRequests,
                    string.Empty)
            };
        }
    }
}