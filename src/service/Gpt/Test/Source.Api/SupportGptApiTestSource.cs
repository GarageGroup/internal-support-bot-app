using System.Text.Json;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

internal static partial class SupportGptApiTestSource
{
    private static string ToJson<T>(this T value)
        =>
        JsonSerializer.Serialize(value);
}