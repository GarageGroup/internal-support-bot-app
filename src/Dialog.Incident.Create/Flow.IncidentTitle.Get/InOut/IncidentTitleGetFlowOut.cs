using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot;

public sealed record IncidentTitleGetFlowOut
{
    public IncidentTitleGetFlowOut([AllowNull] string title) => Title = title ?? string.Empty;

    public string Title { get; }
}
