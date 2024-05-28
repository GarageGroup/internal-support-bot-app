using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetDocumentUrls(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.NextValue(
            GetDocumentUrlsAsync);

    private static ValueTask<IncidentCreateFlowState> GetDocumentUrlsAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (context.FlowState.DocumentIds.IsEmpty || context.FlowState.Documents.IsNotEmpty)
        {
            return new(context.FlowState);
        }

        return InnerGetDocumentUrlsAsync(context, cancellationToken);
    }

    private static ValueTask<IncidentCreateFlowState> InnerGetDocumentUrlsAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState.DocumentIds, cancellationToken)
        .PipeParallel(
            context.GetDocumentAsync, ParallelOption)
        .Pipe(
            documents => context.FlowState with
            {
                Documents = documents,
                PhotoUrls = documents.AsEnumerable().Where(IsPhoto).Select(GetUrl).ToFlatArray()
            });

    private static Task<DocumentState> GetDocumentAsync(
        this IChatFlowContextBase context, string documentId, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            documentId, cancellationToken)
        .Pipe(
            context.Api.GetFileLinkAsync)
        .Pipe(
            ParseDocumentState);

    private static DocumentState ParseDocumentState(ChatFileLink link)
    {
        var paths = link.FilePath.Split('/');

        return new(
            fileName: paths[^1],
            url: link.FileUrl)
        {
            Type = ParseType(paths[0])
        };

        static DocumentType ParseType(string path)
        {
            if (string.Equals("photos", path, StringComparison.InvariantCultureIgnoreCase))
            {
                return DocumentType.Photo;
            }

            if (string.Equals("videos", path, StringComparison.InvariantCultureIgnoreCase))
            {
                return DocumentType.Video;
            }

            return DocumentType.Document;
        }
    }

    private static bool IsPhoto(DocumentState document)
        =>
        document.Type is DocumentType.Photo;

    private static string GetUrl(DocumentState document)
        =>
        document.Url;
}