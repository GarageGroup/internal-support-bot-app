using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class CaseTypeGetHelper
{
    private static readonly IReadOnlyDictionary<Guid, CaseTypeValue> typeCodes;

    private static readonly LookupValueSetOption typeCodeChoiceSet;

    static CaseTypeGetHelper()
    {
        typeCodes = new Dictionary<Guid, CaseTypeValue>()
        {
            [Guid.Parse("61deaa7f-95ed-4915-b8d6-5d09deefc9bb")] = new(1, "Вопрос"),
            [Guid.Parse("9a380544-0c61-4e44-86ff-daba4cf764b3")] = new(2, "Проблема"),
            [Guid.Parse("91da3add-0f51-48f4-aaa8-58a22125dabb")] = new(3, "Запрос")
        };

        typeCodeChoiceSet = new(
            typeCodes.Select(ToLokupValue).ToArray(),
            "Выберите тип обращения",
            LookupValueSetDirection.Horizon);
    }

    internal static LookupValueSetOption GetCaseTypeChoiceSet()
        =>
        typeCodeChoiceSet;

    internal static Optional<CaseTypeValue> GetCaseTypeValueOrAbsent(this LookupValue typeCodeValue)
        =>
        typeCodes.GetValueOrAbsent(typeCodeValue.Id);

    private static LookupValue ToLokupValue(KeyValuePair<Guid, CaseTypeValue> item)
        =>
        new(item.Key, item.Value.Name);
}