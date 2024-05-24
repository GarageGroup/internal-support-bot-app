using System;
using GarageGroup.Infra.Telegram.Bot;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

public static class ContactGetDependency
{
    public static Dependency<IChatCommand<ContactGetCommandIn, ContactGetCommandOut>> UseContactGetCommand(
        this Dependency<ICrmCustomerApi, ICrmContactApi, ContactGetFlowOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IChatCommand<ContactGetCommandIn, ContactGetCommandOut>>(CreateCommand);

        static ContactGetCommand CreateCommand(ICrmCustomerApi customerApi, ICrmContactApi contactApi, ContactGetFlowOption option)
        {
            ArgumentNullException.ThrowIfNull(customerApi);
            ArgumentNullException.ThrowIfNull(contactApi);
            ArgumentNullException.ThrowIfNull(option);

            return new(customerApi, contactApi, option);
        }
    }
}