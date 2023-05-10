using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

public static class IncidentCreateDependency
{
    public static IBotBuilder MapIncidentCreateFlow(
        this Dependency<ISupportApi, ISupportGptApi, IncidentCreateFlowOption> dependency, IBotBuilder botBuilder)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(botBuilder);

        return botBuilder.Use(InnerInvokeAsync);

        ValueTask<Unit> InnerInvokeAsync(IBotContext context, CancellationToken cancellationToken)
            =>
            context.RunAsync(
                dependency.ResolveFirst(context.ServiceProvider),
                dependency.ResolveSecond(context.ServiceProvider),
                dependency.ResolveThird(context.ServiceProvider),
                cancellationToken);
    }
}