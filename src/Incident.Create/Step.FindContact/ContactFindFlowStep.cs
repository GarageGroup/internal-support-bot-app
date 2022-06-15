﻿using GGroupp.Infra.Bot.Builder;
using System;

namespace GGroupp.Internal.Support;

using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

internal static class ContactFindFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> FindContcat(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IContactSetSearchFunc contactSetSearchFunc)
        =>
        chatFlow.AwaitLookupValue(
            contactSetSearchFunc.GetDefaultContactsAsync,
            contactSetSearchFunc.SearchContactsAsync,
            CreateResultMessage,
            MapFlowState);

    private static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue contactValue)
        =>
        $"Контакт: {context.EncodeTextWithStyle(contactValue.Name, BotTextStyle.Bold)}";

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue contactValue)
        => 
        contactValue.IsNotSkipValueOrAbsent().Map(flowState.WithContactValue).OrElse(flowState);

    private static IncidentCreateFlowState WithContactValue(this IncidentCreateFlowState flowState, LookupValue contactValue)
        =>
        flowState with 
        { 
            ContactId = contactValue.Id, 
            ContactFullName = contactValue.Name
        };
}

