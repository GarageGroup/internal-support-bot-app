using System;
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
        =>
        AsyncPipeline.Pipe(
            context, cancellationToken)
        .PipeParallelValue(
            GetPicturesAsync,
            GetDocumentsAsync)
        .Pipe(
            @out => context.FlowState with
            {
                Pictures = @out.Item1,
                Documents = @out.Item2
            });

    private static ValueTask<FlatArray<PictureState>> GetPicturesAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (context.FlowState.PhotoIdSet.IsEmpty || context.FlowState.Pictures.IsNotEmpty)
        {
            return new(context.FlowState.Pictures);
        }

        return AsyncPipeline.Pipe(context.FlowState.PhotoIdSet, cancellationToken).PipeParallel(context.GetPictureAsync, ParallelOption);
    }

    private static ValueTask<FlatArray<DocumentState>> GetDocumentsAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (context.FlowState.DocumentIdSet.IsEmpty || context.FlowState.Documents.IsNotEmpty)
        {
            return new(context.FlowState.Documents);
        }

        return AsyncPipeline.Pipe(context.FlowState.DocumentIdSet, cancellationToken).PipeParallel(context.GetDocumentAsync, ParallelOption);
    }

    private static Task<PictureState> GetPictureAsync(
        this IChatFlowContextBase context, string photoId, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            photoId, cancellationToken)
        .Pipe(
            context.Api.GetFileLinkAsync)
        .Pipe(
            static info => new PictureState(
                fileName: info.FilePath.Split('/')[^1],
                imageUrl: info.FileUrl));

    private static Task<DocumentState> GetDocumentAsync(
        this IChatFlowContextBase context, string documentId, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            documentId, cancellationToken)
        .Pipe(
            context.Api.GetFileLinkAsync)
        .Pipe(
            static info =>
            {
                var pathes = info.FilePath.Split('/');

                return new DocumentState(
                    fileName: pathes[^1],
                    url: info.FileUrl)
                {
                    Type = pathes[0].Equals("videos", StringComparison.InvariantCultureIgnoreCase) ? DocumentType.Video : DocumentType.Document
                };
            });
}