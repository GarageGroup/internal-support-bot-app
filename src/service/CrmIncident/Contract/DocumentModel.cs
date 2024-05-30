using System;

namespace GarageGroup.Internal.Support;

public sealed record class DocumentModel
{
    public DocumentModel(string fileName, string url)
    {
        FileName = fileName.OrEmpty();
        Url = url.OrEmpty();
    }

    public string FileName { get; }

    public string Url { get; }

    public DocumentType Type { get; init; }
}