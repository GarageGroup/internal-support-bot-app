using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot;

public sealed record IncidentCustomerFindFlowOut
{
    public IncidentCustomerFindFlowOut(Guid customerId, [AllowNull] string customerTitle)
    {
        CustomerId = customerId;
        CustomerTitle = customerTitle ?? string.Empty;
    }

    public Guid CustomerId { get; }

    public string CustomerTitle { get; }
}
