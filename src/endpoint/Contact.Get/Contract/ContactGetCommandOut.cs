namespace GarageGroup.Internal.Support;

public sealed record class ContactGetCommandOut
{
    public required CommandCustomer Customer { get; init; }

    public required CommandContact Contact { get; init; }
}