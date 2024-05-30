using System;
using System.Web;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static ContactGetResource;

partial class ContactGetFlowStep
{
    internal static ChatFlow<ContactGetFlowState> ExpectContactConfirmationOrSkip(
        this ChatFlow<ContactGetFlowState> chatFlow)
        =>
        chatFlow.ExpectConfirmationOrSkip(
            CreateContactConfirmationCardOption);

    private static ConfirmationCardOption<ContactGetFlowState>? CreateContactConfirmationCardOption(
        IChatFlowContext<ContactGetFlowState> context)
    {
        if (context.FlowState.ShowConfirmation is false)
        {
            return null;
        }

        return new()
        {
            Entity = new(context.Localizer[ConfirmationHeaderText])
            {
                FieldValues =
                [
                    new(context.Localizer[CustomerFieldName], HttpUtility.HtmlEncode(context.FlowState.Customer?.Name)),
                    new(context.Localizer[ContactFieldName], HttpUtility.HtmlEncode(context.FlowState.Contact?.FullName))
                ]
            },
            Keyboard = new(
                confirmButtonText: context.Localizer[ConfirmButton],
                cancelButtonText: context.Localizer[CancelButton],
                forwardCancellation: ForwardCancellation)
        };

        Result<ContactGetFlowState, ChatBreakState> ForwardCancellation()
            =>
            context.FlowState with
            {
                Customer = null,
                Contact = null,
                ShowConfirmation = false
            };
    }
}