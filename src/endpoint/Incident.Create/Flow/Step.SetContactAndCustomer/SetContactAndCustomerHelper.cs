using GarageGroup.Infra.Bot.Builder;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace GarageGroup.Internal.Support;

internal static class SetContactAndCustomerHelper
{
    internal static ValueTask<IncidentCreateFlowState> SetContactAndCustomersAsync(
        this ICrmContactApi crmContactApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        CancellationToken cancellationToken)
    {
        if (context.FlowState.IsNotFirstLaunch is true)
        {
            return new(context.FlowState);
        }

        return crmContactApi.InnerSetContactAndCustomersAsync(context, cancellationToken);
    }

    private static ValueTask<IncidentCreateFlowState> InnerSetContactAndCustomersAsync(
        this ICrmContactApi crmContactApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static state => new ContactGetIn(
                telegramSenderId: (state.TelegramSender?.Id).OrEmpty()))
        .PipeValue(
            crmContactApi.GetAsync)
        .Fold(
            @out => context.FlowState with
            {
                Contact = new()
                {
                    Id = @out.ContactId,
                    FullName = @out.ContactName
                },
                Customer = new()
                {
                    Id = @out.CustomerId,
                    Title = @out.CustomerName
                }
            },
            _ => context.FlowState);
}