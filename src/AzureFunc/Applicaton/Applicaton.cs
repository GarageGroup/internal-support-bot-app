using System.Net.Http;
using GGroupp.Infra;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

[KeepWarmFunction("KeepBotWarm", "0 */5 * * * *")]
internal static partial class Application
{
    private const string DataverseSectionName = "Dataverse";

    private static Dependency<HttpMessageHandler> UseHttpMessageHandlerStandard(string loggerCategoryName)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler().UseLogging(loggerCategoryName);

    private static Dependency<IDataverseApiClient> UseDataverseApiClient()
        =>
        UseHttpMessageHandlerStandard("DataverseApi").UseDataverseApiClient(DataverseSectionName);
}