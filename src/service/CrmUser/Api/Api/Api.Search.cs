using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmUserApi
{
    public ValueTask<Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>> SearchAsync(
        UserSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe(
            static @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = UserSetSearchEntities,
                Top = @in.Top
            })
        .PipeValue(
            dataverseApi.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            static success => new UserSetSearchOut
            {
                Users = success.Value.Map(MapUserItem)
            });

    private static UserItemOut MapUserItem(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            fullName: item.ExtensionData.AsEnumerable().GetValueOrAbsent("fullname").OrDefault()?.ToString());

    private static UserSetSearchFailureCode MapFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => UserSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => UserSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => UserSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}