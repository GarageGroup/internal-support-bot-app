using System;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;
using IIncidentCreateFlowSender = IQueueWriter<FlowMessage<IncidentCreateFlowMessage>>;

partial class GSupportBotBuilder
{
    internal static IBotBuilder UseGSupportIncidentCreate(this IBotBuilder botBuilder)
        =>
        botBuilder.UseIncidentCreate(
            GetCustomerSetSearchApi,
            GetContactSetSearchApi,
            GetIncidentCreateFlowSender);

    private static ICustomerSetSearchFunc GetCustomerSetSearchApi(IBotContext botContext)
        =>
        CreateStandardHttpHandlerDependency("CustomerSetSearchApi")
        .UseDataverseApiClient()
        .UseCustomerSetSearchApi()
        .Resolve(botContext.ServiceProvider);

    private static IContactSetSearchFunc GetContactSetSearchApi(IBotContext botContext)
        =>
        CreateStandardHttpHandlerDependency("ContactSetSearchApi")
        .UseDataverseApiClient()
        .UseContactSetSearchApi()
        .Resolve(botContext.ServiceProvider);

    private static IIncidentCreateFlowSender GetIncidentCreateFlowSender(IBotContext botContext)
        =>
        botContext.ServiceProvider.GetRequiredService<IConfiguration>().GetValue("Feature:IsServiceBusUsed", false) switch
        {
            true    => UseBusIncidentCreateFlowSender().Resolve(botContext.ServiceProvider),
            _       => UseQueueIncidentCreateFlowSender().Resolve(botContext.ServiceProvider)
        };

    private static Dependency<IIncidentCreateFlowSender> UseQueueIncidentCreateFlowSender()
        =>
        Dependency.From(ResolveIncidentCreateQueueOption).UseQueueWriter<FlowMessage<IncidentCreateFlowMessage>>();

    private static Dependency<IIncidentCreateFlowSender> UseBusIncidentCreateFlowSender()
        =>
        Dependency.From(ResolveIncidentCreateServiceBusQueueOption).UseServiceBusQueueWriter<FlowMessage<IncidentCreateFlowMessage>>();

    private static ServiceBusQueueOption ResolveIncidentCreateServiceBusQueueOption(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredSection("IncidentCreateServiceBusQueue").GetServiceBusQueueOption();

    private static ServiceBusQueueOption GetServiceBusQueueOption(this IConfigurationSection section)
        =>
        new(
            connectionString: section["ConnectionString"],
            name: section["Name"]);

    private static QueueOption ResolveIncidentCreateQueueOption(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredSection("IncidentCreateQueue").GetQueueOption();

    private static QueueOption GetQueueOption(this IConfigurationSection section)
        =>
        new(
            queueConnectionString: section["ConnectionString"],
            queueName: section["Name"],
            visibilityTimeout: section.GetValue<TimeSpan?>("VisibilityTimeout"),
            timeToLive: section.GetValue<TimeSpan?>("TimeToLive"));
}