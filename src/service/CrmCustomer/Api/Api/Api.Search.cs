using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmCustomerApi
{
    public ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>> SearchAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe(
            static @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = CustomerSetSearchEntities,
                Top = @in.Top
            })
        .PipeValue(
            dataverseApi.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            static @out => new CustomerSetSearchOut
            {
                Customers = @out.Value.Map(MapCustomer)
            });

    private static CustomerItemOut MapCustomer(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            title: item.ExtensionData.AsEnumerable().GetValueOrAbsent("name").OrDefault()?.ToString());

    private static CustomerSetSearchFailureCode MapFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => CustomerSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => CustomerSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => CustomerSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}