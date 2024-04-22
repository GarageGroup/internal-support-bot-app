using System;
using System.Collections.Generic;
using System.Linq;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class GptTraceHelper
{
    internal static void TraceGpt(this IncidentCreateFlowOption option, IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (string.IsNullOrEmpty(context.FlowState.Gpt.Title))
        {
            return;
        }

        var isGptUsed = string.Equals(context.FlowState.Gpt.Title, context.FlowState.Title, StringComparison.InvariantCulture);
        var properties = new Dictionary<string, string>(option.GptTraceData.AsEnumerable())
        {
            ["flowId"] = context.ChatFlowId,
            ["event"] = "GptUsage",
            ["isGptUsed"] = isGptUsed ? "true" : "false",
            ["similarityGptByCharacters"] = CalculateSimilarityByCharacters(context.FlowState.Gpt.Title, context.FlowState.Title.OrEmpty()).ToString(),
            ["similarityGptByWords"] = CalculateSimilarityByWords(context.FlowState.Gpt.Title, context.FlowState.Title.OrEmpty()).ToString(),
            ["gptTitle"] = context.FlowState.Gpt.Title,
            ["selectedTitle"] = context.FlowState.Title.OrEmpty(),
            ["sourceDescription"] = context.FlowState.Gpt.SourceMessage.OrEmpty(),
        };

        context.BotTelemetryClient.TrackEvent("CompleteIncidentGptUsage", properties);
    }

    private static double CalculateSimilarityByCharacters(string generatedTitle, string finalTitle)
    {
        var editDistance = new int[generatedTitle.Length + 1, finalTitle.Length + 1];

        for (var i = 0; i <= generatedTitle.Length; i++)
        {
            for (var j = 0; j <= finalTitle.Length; j++)
            {
                if (i == 0)
                {
                    editDistance[i, j] = j;
                }
                else if (j == 0)
                {
                    editDistance[i, j] = i;
                }
                else
                {
                    editDistance[i, j] = Math.Min(
                        Math.Min(editDistance[i - 1, j] + 1, editDistance[i, j - 1] + 1),
                        editDistance[i - 1, j - 1] + (generatedTitle[i - 1] == finalTitle[j - 1] ? 0 : 1));
                }
            }
        }

        var maxLength = Math.Max(generatedTitle.Length, finalTitle.Length);
        var similarity = 1.0 - (double)editDistance[generatedTitle.Length, finalTitle.Length] / maxLength;

        return Math.Round(similarity, 3);
    }

    private static double CalculateSimilarityByWords(string generatedTitle, string finalTitle)
    {
        var wordFreqGenerated = GetWordFrequency(generatedTitle);
        var wordFreqFinal = GetWordFrequency(finalTitle);

        var allWords = new HashSet<string>();
        allWords.UnionWith(wordFreqGenerated.Keys);
        allWords.UnionWith(wordFreqFinal.Keys);

        var vectorGenerated = new List<int>();
        var vectorFinal = new List<int>();

        foreach (var word in allWords)
        {
            vectorGenerated.Add(wordFreqGenerated.ContainsKey(word) ? wordFreqGenerated[word] : 0);
            vectorFinal.Add(wordFreqFinal.ContainsKey(word) ? wordFreqFinal[word] : 0);
        }

        var dotProduct = CalculateDotProduct(vectorGenerated, vectorFinal);
        var magnitudeGenerated = Math.Sqrt(CalculateDotProduct(vectorGenerated, vectorGenerated));
        var magnitudeFinal = Math.Sqrt(CalculateDotProduct(vectorFinal, vectorFinal));

        var similarity = dotProduct / (magnitudeGenerated * magnitudeFinal);
        return Math.Round(similarity, 3);

        static Dictionary<string, int> GetWordFrequency(string title)
        =>
        title.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .GroupBy(word => word.ToLower())
            .ToDictionary(group => group.Key, group => group.Count());

        static double CalculateDotProduct(List<int> firstVector, List<int> secondVector)
        {
            var result = 0;
            for (int i = 0; i < firstVector.Count; i++)
            {
                result += firstVector[i] * secondVector[i];
            }
            return result;
        }
    }
}