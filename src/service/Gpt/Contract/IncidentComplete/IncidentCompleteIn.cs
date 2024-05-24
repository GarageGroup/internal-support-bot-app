using System;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCompleteIn
{
    public IncidentCompleteIn(string? message, FlatArray<string> imageUrl)
    {
        Message = message;
        ImageUrls = imageUrl;
    }        

    public string? Message { get; }

    public FlatArray<string> ImageUrls { get; }
}