using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal static class CustomerChooseActivity
{
    internal const int MaxCustomerSetCount = 5;

    public static IActivity CreateCustomerChooseActivity(this Activity activity, IReadOnlyCollection<CustomerItemFindOut> customers)
        =>
        new HeroCard
        {
            Title = "Выберите клиента",
            Buttons = customers.Take(MaxCustomerSetCount).Select(activity.CreateCustomerAction).ToArray()
        }
        .ToAttachment()
        .ToActivity();

    public static Result<IncidentCustomerFindFlowOut, Unit> GetCustomerOrAbsent(this Activity activity)
    {
        if (activity.IsMessageType() is false)
        {
            return default;
        }

        if (activity.IsCardSupported())
        {
            var json = activity.Value.ToStringOrEmpty();
            if(string.IsNullOrEmpty(json))
            {
                return default;
            }

            var value = JsonConvert.DeserializeObject<CustomerValueJson>(json);
            if (value.CustomerId is null)
            {
                return default;
            }

            return new IncidentCustomerFindFlowOut(value.CustomerId.Value, value.CustomerTitle);
        }

        return activity.GetGuidOrAbsent().ToResult().MapSuccess(
            customerId => new IncidentCustomerFindFlowOut(customerId, default));
    }

    private static CardAction CreateCustomerAction(this Activity activity, CustomerItemFindOut customer)
        =>
        new(ActionTypes.PostBack)
        {
            Title = customer.Title,
            Text = customer.Title,
            Value = activity.IsCardSupported()
                ? new CustomerValueJson { CustomerId = customer.Id, CustomerTitle = customer.Title }
                : customer.Id
        };

    private sealed record CustomerValueJson
    {
        [JsonProperty("customerId")]
        public Guid? CustomerId { get; init; }

        [JsonProperty("customerTitle")]
        public string? CustomerTitle { get; init; }
    }
}

