using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

public static class IncidentCreateDependency
{
    public static IBotBuilder MapIncidentCreateFlow(
        this Dependency<IncidentCreateFlowOption, ICrmCustomerApi, ICrmContactApi, ICrmUserApi, ICrmIncidentApi, ISupportGptApi> dependency,
        IBotBuilder botBuilder)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(botBuilder);

        return botBuilder.Use(InnerInvokeAsync);

        ValueTask<Unit> InnerInvokeAsync(IBotContext context, CancellationToken cancellationToken)
            =>
            context.RunAsync(
                option: dependency.ResolveFirst(context.ServiceProvider),
                crmCustomerApi: dependency.ResolveSecond(context.ServiceProvider),
                crmContactApi: dependency.ResolveThird(context.ServiceProvider),
                crmUserApi: dependency.ResolveFourth(context.ServiceProvider),
                crmIncidentApi: dependency.ResolveFifth(context.ServiceProvider),
                supportGptApi: dependency.ResolveSixth(context.ServiceProvider),
                cancellationToken: cancellationToken);
    }
}