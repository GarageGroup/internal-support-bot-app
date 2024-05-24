namespace GarageGroup.Internal.Support;

internal sealed partial class ContactGetCommand : IContactGetCommand
{
    private readonly ICrmCustomerApi crmCustomerApi;

    private readonly ICrmContactApi crmContactApi;

    private readonly ContactGetFlowOption option;

    internal ContactGetCommand(ICrmCustomerApi crmCustomerApi, ICrmContactApi crmContactApi, ContactGetFlowOption option)
    {
        this.crmCustomerApi = crmCustomerApi;
        this.crmContactApi = crmContactApi;
        this.option = option;
    }
}