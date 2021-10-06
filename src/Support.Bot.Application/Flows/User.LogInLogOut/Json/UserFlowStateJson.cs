using System;

namespace GGroupp.Internal.Support.Bot
{
    internal sealed record UserFlowStateJson
    {
        public Guid UserId { get; init; }
    }
}