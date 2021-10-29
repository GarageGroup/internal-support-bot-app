using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GGroupp.Internal.Support.Bot;

internal static class CustomerChooseActivity
{
    internal const int MaxCustomerSetCount = 5;

    internal const string SearchDictName = "CustomerSearchResultDictionary";
    public static IActivity CreateCustomerChooseActivity(this Activity activity, IReadOnlyCollection<CustomerItemFindOut> customers)
        =>
        new HeroCard
        {
            Title = "Выберите клиента",
            Buttons = customers.Take(MaxCustomerSetCount).Select(activity.CreateCustomerAction).ToArray()
        }
        .ToAttachment()
        .ToActivity();

    public static Result<IncidentCustomerFindFlowOut, Unit> GetCustomerOrAbsent(this DialogContext dialogContext)
    {
        var activity = dialogContext.Context.Activity;

        if (activity.IsMessageType() is false)
        {
            return default;
        }

        var customerGuidOp = activity.GetGuidOrAbsent();
        if (customerGuidOp.IsAbsent)
        {
            return default;
        }

        var customerGuid = customerGuidOp.OrThrow();
        
        if(dialogContext.ActiveDialog.State is null || dialogContext.ActiveDialog.State.ContainsKey(SearchDictName) is false)
        {
            return default;
        }

        var searchDict = (Dictionary<Guid,string>)dialogContext.ActiveDialog.State[SearchDictName];

        if(searchDict is null || searchDict.ContainsKey(customerGuid) is false)
        {
            return default;
        }

        return new IncidentCustomerFindFlowOut(customerGuid, searchDict[customerGuid]);
    }

    private static CardAction CreateCustomerAction(this Activity activity, CustomerItemFindOut customer)
        =>
        new(ActionTypes.PostBack)
        {
            Title = customer.Title,
            Text = customer.Id.ToString("D", CultureInfo.InvariantCulture),
            Value = customer.Id.ToString("D", CultureInfo.InvariantCulture)
        };

    private sealed record CustomerValueJson
    {
        [JsonProperty("customerId")]
        public Guid? CustomerId { get; init; }

        [JsonProperty("customerTitle")]
        public string? CustomerTitle { get; init; }
    }
}

