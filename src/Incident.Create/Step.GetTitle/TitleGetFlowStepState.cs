using System;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support;

internal sealed record class TitleGetFlowStepState
{
    [JsonProperty("buttonId")]
    public Guid ButtonId { get; init; }

    [JsonProperty("offeredTitle")]
    public string? OfferedTitle { get; init; }
}