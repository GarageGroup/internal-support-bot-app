using System;

namespace GarageGroup.Internal.Support;

internal sealed record class AnnotationFailureState
{
    public AnnotationFailureState(string? fileName)
        =>
        FileName = fileName.OrEmpty();
    
    public string? FileName { get; }

    public IncidentCreateFailureCode? FailureCode { get; init; }
}
