using GarageGroup.Infra;
using System;

namespace GarageGroup.Internal.Support;

internal sealed partial class SupportGptApi(IHttpApi gptHttp, SupportGptApiOption option) : ISupportGptApi
{
    private static string? TrimTitle(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        var title = source.Trim('.').Trim();
        if (title.StartsWith('"') && title.EndsWith('"'))
        {
            title = title[1..^1];
        }

        return title.Trim('.').Trim().OrNullIfEmpty();
    }
}