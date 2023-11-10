using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmContactApi
{
    public ValueTask<Result<ContactSetSearchOut, Failure<ContactSetGetFailureCode>>> SearchAsync(
        ContactSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe(
            static @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = ContactSetSearchEntities,
                Filter = new DataverseComparisonFilter("parentcustomerid", DataverseComparisonOperator.Equal, @in.CustomerId),
                Top = @in.Top 
            })
        .PipeValue(
            dataverseApi.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            static @out => new ContactSetSearchOut
            {
                Contacts = @out.Value.Map(MapDataverseSearchItem)
            });

    private static ContactItemOut MapDataverseSearchItem(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            fullName: item.ExtensionData.AsEnumerable().GetValueOrAbsent("fullname").OrDefault()?.ToString());

    private static ContactSetGetFailureCode MapFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => ContactSetGetFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => ContactSetGetFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => ContactSetGetFailureCode.TooManyRequests,
            _ => default
        };
}