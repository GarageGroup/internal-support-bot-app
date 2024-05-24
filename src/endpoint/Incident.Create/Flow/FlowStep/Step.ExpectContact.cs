using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectContact(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.RunCommand<ContactGetCommandIn, ContactGetCommandOut>(
            CreateContactGetInput,
            MapContact);

    private static ContactGetCommandIn CreateContactGetInput(IChatFlowContext<IncidentCreateFlowState> context)
    {
        var state = context.FlowState;
        if (state.IsRepeated is false)
        {
            return new(
                telegramSender: state.SourceSender is null ? null : new(state.SourceSender.UserId));
        }

        return new(
            customer: state.Customer is null ? null : new(state.Customer.Id, state.Customer.Title),
            contact: state.Contact is null ? null : new(state.Contact.Id, state.Contact.FullName));
    }

    private static IncidentCreateFlowState MapContact(IChatFlowContext<IncidentCreateFlowState> context, ContactGetCommandOut contact)
        =>
        context.FlowState with
        {
            Customer = new(contact.Customer.Id, contact.Customer.Title),
            Contact = new(contact.Contact?.Id, contact.Contact?.FullName)
        };
}