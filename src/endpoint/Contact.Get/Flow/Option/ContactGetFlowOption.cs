namespace GarageGroup.Internal.Support;

public sealed record class ContactGetFlowOption
{
    public required int DbRequestPeriodInDays { get; init; }
}