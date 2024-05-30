using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCompleteIn
{
    public IncidentCompleteIn([AllowNull] string message, FlatArray<string> imageUrls)
    {
        Message = message.OrNullIfWhiteSpace();
        ImageUrls = imageUrls;
    }

    public string? Message { get; }

    public FlatArray<string> ImageUrls { get; }
}