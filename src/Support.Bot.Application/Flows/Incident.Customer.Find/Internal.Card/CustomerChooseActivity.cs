using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support.Bot;

internal static class CustomerChooseActivity
{
    public static IActivity CreateCustomerChooseActivity(this ITurnContext turnContext, IEnumerable<CustomerItemFindOut> customers)
        =>
        new HeroCard
        {
            Title = "Выберите клиента",
            Buttons = customers.Select(CreateCustomerAction).ToArray()
        }
        .ToAttachment()
        .Pipe(
            turnContext.Activity.CreateReplyWithAttachment);

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

