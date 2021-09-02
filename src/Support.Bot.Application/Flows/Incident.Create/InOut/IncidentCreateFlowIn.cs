using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot
{
    public sealed record IncidentCreateFlowIn
    {
        public IncidentCreateFlowIn(Guid ownerId, Guid customerId, [AllowNull] string title, [AllowNull] string description)
        {
            OwnerId = ownerId;
            CustomerId = customerId;
            Title = title ?? string.Empty;
            Description = description ?? string.Empty;
        }

        public Guid OwnerId { get; }

        public Guid CustomerId { get; }

        public string Title { get; }

        public string Description { get; }
    }
}