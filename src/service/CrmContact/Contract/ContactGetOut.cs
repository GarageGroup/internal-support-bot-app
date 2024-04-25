using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class ContactGetOut
{
    public ContactGetOut(
        Guid contactId, 
        [AllowNull] string contactName, 
        Guid customerId, 
        [AllowNull] string customerName)
    {
        ContactId = contactId;
        ContactName = contactName.OrEmpty();
        CustomerId = customerId;
        CustomerName = customerName.OrEmpty();
    }

    public Guid ContactId { get; }

    public string ContactName { get; }

    public Guid CustomerId { get; }

    public string CustomerName { get; }
}