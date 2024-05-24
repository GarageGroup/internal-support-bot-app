using System;

namespace GarageGroup.Internal.Support;

internal sealed record class PictureState
{
    public PictureState(string fileName, string imageUrl)
    {
        FileName = fileName.OrEmpty();
        ImageUrl = imageUrl.OrEmpty();
    }
    public string FileName { get; }

    public string ImageUrl { get; }
}