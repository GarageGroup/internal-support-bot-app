using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support.Bot;

internal static class CustomerChooseAdaptiveCard
{
    public static IActivity CreateCustomerChooseActivity(this DialogContext dialogContext, IEnumerable<CustomerItemFindOut> customers)
        =>
        new AdaptiveCardJson("1.3")
        {
            Type = "AdaptiveCard",
            Body = new object[]
            {
                new
                {
                    type = "TextBlock",
                    size = "Medium",
                    weight = "Bolder",
                    text = "Выберите клиента"
                }
            },
            Actions = customers.Select(CreateCustomerAction).ToArray()
        }
        .Pipe(
            dialogContext.Context.Activity.CreateReplyFromCard);

    private static object CreateCustomerAction(CustomerItemFindOut customer)
        =>
        new
        {
            type = "Action.Submit",
            title = customer.Title,
            data = new CustomerChooseDataJson
            {
                Id = customer.Id,
                Title = customer.Title
            }
        };
}

