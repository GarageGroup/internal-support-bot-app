using GarageGroup.Infra;
using System;
using System.Net.Mime;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<HttpSendFailure, Failure<IncidentCompleteFailureCode>> OutputFailureTestData
        =>
        new()
        {
            {
                default,
                Failure.Create(IncidentCompleteFailureCode.Unknown, "An unexpected http failure occured: 0.")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.BadRequest,
                    Body = new()
                    {
                        Type = new(MediaTypeNames.Application.Json),
                        Content = BinaryData.FromString("Some failure message")
                    }
                },
                Failure.Create(IncidentCompleteFailureCode.Unknown, "An unexpected http failure occured: 400.\nSome failure message")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.InternalServerError,
                    ReasonPhrase = "Some reason",
                    Headers =
                    [
                        new("SomeHeader", "Some value")
                    ],
                    Body = new()
                    {
                        Content = BinaryData.FromString("Some error text.")
                    }
                },
                Failure.Create(IncidentCompleteFailureCode.Unknown, "An unexpected http failure occured: 500 Some reason.\nSome error text.")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.TooManyRequests,
                    ReasonPhrase = "Some reason",
                    Headers =
                    [
                        new("SomeHeader", "Some value")
                    ],
                    Body = new()
                    {
                        Content = BinaryData.FromString("Some error text.")
                    }
                },
                Failure.Create(IncidentCompleteFailureCode.TooManyRequests, "An unexpected http failure occured: 429 Some reason.\nSome error text.")
            },
        };
}