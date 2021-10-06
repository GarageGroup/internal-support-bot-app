using System;

namespace GGroupp.Internal.Support.Bot
{
    public sealed record IncidentCustomerFindFlowOut
    {
        public IncidentCustomerFindFlowOut(Guid customerId)
            =>
            CustomerId = customerId;

        public Guid CustomerId { get; }
    }
}