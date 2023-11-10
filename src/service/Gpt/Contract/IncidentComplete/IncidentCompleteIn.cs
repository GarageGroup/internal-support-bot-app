using System;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCompleteIn
{
    public IncidentCompleteIn(string message)
        =>
        Message = message.OrEmpty();

    public string Message { get; }
}