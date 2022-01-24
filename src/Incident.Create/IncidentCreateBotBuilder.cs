using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

public static class IncidentCreateBotBuilder
{
    public static IBotBuilder UseIncidentCreate(
        this IBotBuilder botBuilder,
        Func<IBotContext, IncidentCreateBotOption> optionResolver,
        Func<IBotContext, ICustomerSetSearchFunc> customerSetSearchFuncResolver,
        Func<IBotContext, IIncidentCreateFunc> incidentCreateFuncResolver,
        Func<IBotContext, IContactSetSearchFunc> contactSetSearchFuncResolver)
        =>
        InnerUseIncidentCreate(
            botBuilder ?? throw new ArgumentNullException(nameof(botBuilder)),
            optionResolver ?? throw new ArgumentNullException(nameof(optionResolver)),
            customerSetSearchFuncResolver ?? throw new ArgumentNullException(nameof(customerSetSearchFuncResolver)),
            incidentCreateFuncResolver ?? throw new ArgumentNullException(nameof(incidentCreateFuncResolver)),
            contactSetSearchFuncResolver ?? throw new ArgumentNullException(nameof(contactSetSearchFuncResolver)));

    private static IBotBuilder InnerUseIncidentCreate(
        IBotBuilder botBuilder,
        Func<IBotContext, IncidentCreateBotOption> optionResolver,
        Func<IBotContext, ICustomerSetSearchFunc> customerSetSearchFuncResolver,
        Func<IBotContext, IIncidentCreateFunc> incidentCreateFuncResolver,
        Func<IBotContext, IContactSetSearchFunc> contactSetSearchFuncResolver)
    {
        return botBuilder.Use(InnerInvokeAsync);

        ValueTask<Unit> InnerInvokeAsync(IBotContext botContext, CancellationToken cancellationToken)
        {
            return botContext.InternalRecoginzeOrFailure().FoldValueAsync(InvokeFlowAsync, FinishAsync);

            ValueTask<Unit> InvokeFlowAsync(ChatFlow chatFlow)
                =>
                chatFlow.InvokeFlow(
                    optionResolver.Invoke(botContext),
                    customerSetSearchFuncResolver.Invoke(botContext),
                    incidentCreateFuncResolver.Invoke(botContext),
                    contactSetSearchFuncResolver.Invoke(botContext),
                    botContext.LoggerFactory,
                    botContext.BotUserProvider)
                .CompleteValueAsync(cancellationToken);
        }

        static ValueTask<Unit> FinishAsync(Unit _)
            =>
            default;
    }
}