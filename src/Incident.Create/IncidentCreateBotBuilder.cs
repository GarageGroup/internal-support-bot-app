using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

public static class IncidentCreateBotBuilder
{
    public static IBotBuilder UseIncidentCreate(
        this IBotBuilder botBuilder,
        Func<IBotContext, IncidentCreateBotOption> optionResolver,
        Func<IBotContext, ICustomerSetSearchFunc> customerSetSearchFuncResolver,
        Func<IBotContext, IContactSetSearchFunc> contactSetSearchFuncResolver,
        Func<IBotContext, IIncidentCreateFunc> incidentCreateFuncResolver)
    {
        _ = botBuilder ?? throw new ArgumentNullException(nameof(botBuilder));
        _ = optionResolver ?? throw new ArgumentNullException(nameof(optionResolver));
        _ = customerSetSearchFuncResolver ?? throw new ArgumentNullException(nameof(customerSetSearchFuncResolver));
        _ = contactSetSearchFuncResolver ?? throw new ArgumentNullException(nameof(contactSetSearchFuncResolver));
        _ = incidentCreateFuncResolver ?? throw new ArgumentNullException(nameof(incidentCreateFuncResolver));

        return botBuilder.Use(InnerInvokeAsync);

        ValueTask<Unit> InnerInvokeAsync(IBotContext botContext, CancellationToken cancellationToken)
        {
            return botContext.InternalRecoginzeOrFailure().FoldValueAsync(InvokeFlowAsync, NextAsync);

            ValueTask<Unit> InvokeFlowAsync(ChatFlow chatFlow)
                =>
                chatFlow.Start(
                    optionResolver.Invoke(botContext),
                    customerSetSearchFuncResolver.Invoke(botContext),
                    contactSetSearchFuncResolver.Invoke(botContext),
                    incidentCreateFuncResolver.Invoke(botContext))
                .CompleteValueAsync(cancellationToken);

            ValueTask<Unit> NextAsync(Unit _)
                =>
                botContext.BotFlow.NextAsync(cancellationToken);
        }
    }
}