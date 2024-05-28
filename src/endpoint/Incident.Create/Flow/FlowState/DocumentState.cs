using System;

namespace GarageGroup.Internal.Support;

internal sealed record class DocumentState
{
    public DocumentState(string fileName, string url)
    {
        FileName = fileName.OrEmpty();
        Url = url.OrEmpty();
    }

    public string FileName { get; }

    public string Url { get; }

    public DocumentType Type { get; init; }
}