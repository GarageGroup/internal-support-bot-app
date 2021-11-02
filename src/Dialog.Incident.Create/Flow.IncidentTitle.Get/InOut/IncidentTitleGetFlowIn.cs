using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot;

public sealed record IncidentTitleGetFlowIn
{
    public IncidentTitleGetFlowIn([AllowNull] string description) => Description = description ?? string.Empty;
    
    public string Description { get; }
}