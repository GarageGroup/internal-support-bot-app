using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

partial class IncidentCreateDialogFlowFunc
{
    public ValueTask<ChatFlowStepResult<Unit>> InvokeAsync(DialogContext dialogContext, Unit _, CancellationToken cancellationToken = default)
        =>
        ChatFlow.Start(
            dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)))
        .MapFlowState(
            _ => new IncidentCreateFlowState
            {
                Description = dialogContext.Context.Activity.Text
            })
        .ForwardChildValue(
            Unit.From,
            userLogInFlowFunc.InvokeAsync,
            (incident, userData) => incident with
            {
                OwnerId = userData.UserId
            })
        .ForwardChildValue(
            Unit.From,
            incidentCustomerFindFlowFunc.InvokeAsync,
            (incident, customer) => incident with
            {
                CustomerId = customer.CustomerId
            })
        .ForwardChildValue(
            incident => new IncidentTitleGetFlowIn(
                description: incident.Description ?? string.Empty),
            incidentTitleFlowFunc.InvokeAsync,
            (incident, title) => incident with
            {
                Title = title.Title
            })
        .ForwardChildValue(
            incident => new IncidentCreateFlowIn(
                ownerId: incident.OwnerId,
                customerId: incident.CustomerId,
                title: incident.Title,
                description: incident.Description),
            incidentCreateFlowFunc.InvokeAsync)
        .CompleteValueAsync(
            cancellationToken);
}
