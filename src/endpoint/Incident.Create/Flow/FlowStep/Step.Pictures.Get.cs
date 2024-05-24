using GarageGroup.Infra.Telegram.Bot;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetPictures(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.NextValue(
            InnerGetPictures);

    private static async ValueTask<IncidentCreateFlowState> InnerGetPictures(
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
}