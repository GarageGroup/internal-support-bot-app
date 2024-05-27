using System;

namespace GarageGroup.Internal.Support;

public sealed record class PictureModel
{
    public PictureModel(string fileName, string imageUrl)
    {
        FileName = fileName.OrEmpty();
        ImageUrl = imageUrl.OrEmpty();
    }

    public string FileName { get; }

    public string ImageUrl { get; }
}