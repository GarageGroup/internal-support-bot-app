using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class CaseTypeGetFlowStep
{
    private static readonly IReadOnlyDictionary<Guid, (int code, string)> typeCodes;

    private static readonly LookupValueSetSeachOut typeCodeChoiceSet;

    static CaseTypeGetFlowStep()
    {
        typeCodes = new Dictionary<Guid, (int, string)>()
        {
            [Guid.Parse("61deaa7f-95ed-4915-b8d6-5d09deefc9bb")] = (1, "Вопрос"),
            [Guid.Parse("9a380544-0c61-4e44-86ff-daba4cf764b3")] = (2, "Проблема"),
            [Guid.Parse("91da3add-0f51-48f4-aaa8-58a22125dabb")] = (3, "Запрос")
        };
        typeCodeChoiceSet = new(typeCodes.Select(ToLokupValue).ToArray(), "Выберите тип обращения", LookupValueSetDirection.Horizon);
    }

    internal static ChatFlow<IncidentCreateFlowState> GetCaseType(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitChoiceValue(
            static _ => typeCodeChoiceSet,
            (flowState, typeCodeValue) => flowState with
            {
                CaseTypeCode = typeCodes.GetValueOrDefault(typeCodeValue.Id).code,
                CaseTypeTitle = typeCodeValue.Name
            });

    private static LookupValue ToLokupValue(KeyValuePair<Guid, (int, string name)> item)
        =>
        new(item.Key, item.Value.name);
}