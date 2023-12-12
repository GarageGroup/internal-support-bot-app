using System;
using System.Net;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<HttpStatusCode, string?, Failure<IncidentCompleteFailureCode>> OutputFailureTestData
    {
        get
        {
            var data = new TheoryData<HttpStatusCode, string?, Failure<IncidentCompleteFailureCode>>();

            var firstJson = new StubGptJsonOut
            {
                Choices =
                [
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
                ]
            }
            .ToJson();

            data.Add(
                HttpStatusCode.OK,
                firstJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"An unexpected GPT finish reason: 'failed'. Body: '{firstJson}'"));

            var secondJson = new StubGptJsonOut
            {
                Choices = []
            }
            .ToJson();

            data.Add(
                HttpStatusCode.OK,
                secondJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"GPT result choices are absent. Body: '{secondJson}'"));

            var thirdJson = new StubGptJsonOut
            {
                Choices = null
            }
            .ToJson();

            data.Add(
                HttpStatusCode.OK,
                thirdJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"GPT result choices are absent. Body: '{thirdJson}'"));

            var fourthJson = new StubFailureJson
            {
                Error = new()
                {
                    Message = "Some error message"
                }
            }
            .ToJson();

            data.Add(
                HttpStatusCode.BadRequest,
                fourthJson,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    $"An unexpected http status code: BadRequest. Body: '{fourthJson}'"));

            data.Add(
                HttpStatusCode.Unauthorized,
                "Some error content",
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    "An unexpected http status code: Unauthorized. Body: 'Some error content'"));

            data.Add(
                HttpStatusCode.NotFound,
                null,
                Failure.Create(
                    IncidentCompleteFailureCode.Unknown,
                    "An unexpected http status code: NotFound. Body: ''"));

            data.Add(
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
                    "Some error message"));

            var fifthJson = new StubFailureJson
            {
                Error = new()
                {
                    Message = null
                }
            }
            .ToJson();

            data.Add(
                HttpStatusCode.TooManyRequests,
                fifthJson,
                Failure.Create(
                    IncidentCompleteFailureCode.TooManyRequests,
                    fifthJson));

            data.Add(
                HttpStatusCode.TooManyRequests,
                null,
                Failure.Create(
                    IncidentCompleteFailureCode.TooManyRequests,
                    string.Empty));

            return data;
        }
    }
}