using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support.Bot;

internal static class CustomerChooseActivity
{
    public static IActivity Create(IEnumerable<CustomerItemFindOut> customers)
        =>
        new HeroCard
        {
            Title = "Выберите клиента",
            Buttons = customers.Select(CreateCustomerAction).ToArray()
        }
        .ToAttachment()
        .ToActivity();

    private static CardAction CreateCustomerAction(CustomerItemFindOut customer)
        =>
        new(ActionTypes.PostBack)
        {
            Title = customer.Title,
            Value = new CustomerChooseValueJson
            {
                Id = customer.Id,
                Title = customer.Title
            }
        };
}

