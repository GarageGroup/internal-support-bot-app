using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot;

internal sealed record IncidentLink
{
    public IncidentLink([AllowNull] string title, [AllowNull] string url)
    {
        Title = title ?? string.Empty;
        Url = url ?? string.Empty;
    }

    public string Title { get; }

    public string Url { get; }
}

