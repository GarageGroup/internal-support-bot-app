using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmOwnerApi
{
    public ValueTask<Result<OwnerSetSearchOut, Failure<OwnerSetGetFailureCode>>> SearchAsync(
        OwnerSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe(
            static @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = OwnerSetSearchEntities,
                Top = @in.Top
            })
        .PipeValue(
            dataverseApi.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            static success => new OwnerSetSearchOut
            {
                Owners = success.Value.Map(MapOwnerItem)
            });

    private static OwnerItemOut MapOwnerItem(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            fullName: item.ExtensionData.AsEnumerable().GetValueOrAbsent("fullname").OrDefault()?.ToString());

    private static OwnerSetGetFailureCode MapFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => OwnerSetGetFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => OwnerSetGetFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => OwnerSetGetFailureCode.TooManyRequests,
            _ => default
        };
}