using GarageGroup.Infra.Telegram.Bot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetDocumentUrls(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.NextValue(
            GetPictures)
        .NextValue(
            GetDocuments);

    private static async ValueTask<IncidentCreateFlowState> GetPictures(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (context.FlowState.PhotoIdSet.IsEmpty)
        {
            return context.FlowState;
        }
        
        var photoInfo = await context.Api.GetFileLinkAsync(context.FlowState.PhotoIdSet[^1], cancellationToken).ConfigureAwait(false);
        
        return context.FlowState with
        {
            Pictures = new FlatArray<PictureState>(
                new PictureState(
                    fileName: photoInfo.FilePath.Split('/')[^1],
                    imageUrl: photoInfo.FileUrl))
        };
    }

    private static async ValueTask<IncidentCreateFlowState> GetDocuments(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (context.FlowState.DocumentIdSet.IsEmpty)
        {
            return context.FlowState;
        }

        var documentList = new List<DocumentState>();
        foreach(var documentId in context.FlowState.DocumentIdSet)
        {
            var documentInfo = await context.Api.GetFileLinkAsync(documentId, cancellationToken).ConfigureAwait(false);

            documentList.Add(new DocumentState(
                fileName: documentInfo.FilePath.Split('/')[^1],
                url: documentInfo.FileUrl)
            {
                Type = documentInfo.FilePath.Split('/')[0].Equals("videos") ? DocumentType.Video : DocumentType.Document
            });
        }

        return context.FlowState with
        {
            Documents = documentList
        };
    }
}