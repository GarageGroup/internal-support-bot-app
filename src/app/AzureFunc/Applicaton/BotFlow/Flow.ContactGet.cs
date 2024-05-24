using System;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Configuration;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static BotCommandBuilder WithContactGetCommand(this BotCommandBuilder builder)
        =>
        builder.With(
            UseContactGetCommand());

    private static Dependency<IChatCommand<ContactGetCommandIn, ContactGetCommandOut>> UseContactGetCommand()
        =>
        Pipeline.Pipe(
            UseDataverseApi().With(UseSqlApi()).UseCrmCustomerApi())
        .With(
            UseDataverseApi().With(UseSqlApi()).UseCrmContactApi())
        .With(
            ResolveContactGetFlowOption)
        .UseContactGetCommand();

    private static ContactGetFlowOption ResolveContactGetFlowOption(IServiceProvider serviceProvider)
        =>
        new()
        {
            DbRequestPeriodInDays = serviceProvider.GetConfiguration().GetValue("DbRequestPeriodInDays", 90)
        };
}