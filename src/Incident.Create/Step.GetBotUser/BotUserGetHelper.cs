using System;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class BotUserGetHelper
{
    internal const string UnexpectedErrorUserMessage
        =
        "Произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее";

    internal static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, DataverseUserData user)
        =>
        flowState with
        { 
            BotUserId = user.SystemUserId, 
            BotUserName = user.FullName.OrNullIfEmpty() ?? user.BotUser.DisplayName
        };
}