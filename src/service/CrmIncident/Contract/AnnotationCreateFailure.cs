using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class AnnotationCreateFailure
{
    public AnnotationCreateFailure([AllowNull] string fileName, [AllowNull] string failureMessage)
    {
        FileName = fileName.OrEmpty();
        FailureMessage = failureMessage.OrEmpty();
    }

    public string FileName { get; }

    public string FailureMessage { get; }

    public AnnotationCreateFailureCode FailureCode { get; set; }

    public Exception? SourceException { get; init; }
}