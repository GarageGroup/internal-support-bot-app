using System.Globalization;

namespace GGroupp.Infra;

internal static partial class DialogContextExtensions
{
    private const string ChatFlowLevelParamName = "_chat_flow__level";

    private const string ChatFlowPositionParamNameTemplate = "_chat_flow_{0}_position";

    private const string ChatFlowStateParamNameTemplate = "_chat_flow_{0}_state";

    private static string GetChatFlowPositionParamName(int flowLevel)
        =>
        string.Format(CultureInfo.InvariantCulture, ChatFlowPositionParamNameTemplate, flowLevel);

    private static string GetChatFlowStateParamName(int flowLevel)
        =>
        string.Format(CultureInfo.InvariantCulture, ChatFlowStateParamNameTemplate, flowLevel);
}