using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
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
        .Forward(
            state => state.Description.IsNotNullOrEmpty()
                ? ChatFlowStepResult.Next(state)
                : ChatFlowStepResult.Interrupt())
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
                CustomerId = customer.CustomerId,
                CustomerTitle = customer.CustomerTitle
            })
        .ForwardChildValue(
            incident => new IncidentTitleGetFlowIn(
                description: incident.Description.OrEmpty()),
            incidentTitleFlowFunc.InvokeAsync,
            (incident, title) => incident with
            {
                Title = title.Title
            })
        .ForwardChildValue(
            Unit.From,
            incidentTypeGetFlowFunc.InvokeAsync,
            (incident, type) => incident with
            {
                CaseType = type.CaseTypeCode
            })
        .ForwardChildValue(
            incident => new IncidentCreateFlowIn(
                ownerId: incident.OwnerId,
                customerId: incident.CustomerId,
                customerTitle: incident.CustomerTitle,
                title: incident.Title,
                caseTypeCode: incident.CaseType,
                description: incident.Description),
            incidentCreateFlowFunc.InvokeAsync)
        .CompleteValueAsync(
            cancellationToken);
}
