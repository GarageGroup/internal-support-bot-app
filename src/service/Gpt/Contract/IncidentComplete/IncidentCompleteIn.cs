namespace GarageGroup.Internal.Support;

public sealed record class IncidentCompleteIn
{
    public IncidentCompleteIn(string message)
        =>
        Message = message ?? string.Empty;

    public string Message { get; }
}