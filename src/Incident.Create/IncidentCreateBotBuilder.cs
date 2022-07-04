using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;
using IIncidentCreateFlowSender = IQueueWriter<FlowMessage<IncidentCreateFlowMessage>>;

public static class IncidentCreateBotBuilder
{
    public static IBotBuilder UseIncidentCreate(
        this IBotBuilder botBuilder,
        Func<IBotContext, ICustomerSetSearchFunc> customerSetSearchFuncResolver,
        Func<IBotContext, IContactSetSearchFunc> contactSetSearchFuncResolver,
        Func<IBotContext, IIncidentCreateFlowSender> incidentCreateFlowSenderResolver)
    {
        _ = botBuilder ?? throw new ArgumentNullException(nameof(botBuilder));
        _ = customerSetSearchFuncResolver ?? throw new ArgumentNullException(nameof(customerSetSearchFuncResolver));
        _ = contactSetSearchFuncResolver ?? throw new ArgumentNullException(nameof(contactSetSearchFuncResolver));
        _ = incidentCreateFlowSenderResolver ?? throw new ArgumentNullException(nameof(incidentCreateFlowSenderResolver));

        return botBuilder.Use(InnerInvokeAsync);

        ValueTask<Unit> InnerInvokeAsync(IBotContext botContext, CancellationToken cancellationToken)
        {
            return botContext.InternalRecoginzeOrFailure().FoldValueAsync(InvokeFlowAsync, NextAsync);

            ValueTask<Unit> InvokeFlowAsync(ChatFlow chatFlow)
                =>
                chatFlow.Start(
                    customerSetSearchFuncResolver.Invoke(botContext),
                    contactSetSearchFuncResolver.Invoke(botContext),
                    incidentCreateFlowSenderResolver.Invoke(botContext))
                .CompleteValueAsync(cancellationToken);

            ValueTask<Unit> NextAsync(Unit _)
                =>
                botContext.BotFlow.NextAsync(cancellationToken);
        }
    }
}