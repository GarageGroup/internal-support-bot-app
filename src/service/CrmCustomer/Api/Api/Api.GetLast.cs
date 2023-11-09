using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmCustomerApi
{
    public ValueTask<Result<LastCustomerSetGetOut, Failure<CustomerSetGetFailureCode>>> GetLastAsync(
        LastCustomerSetGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe<IDbQuery>(
            static @in => DbIncidentCustomer.QueryAll with
            {
                Top = @in.Top,
                SelectedFields = DbIncidentCustomer.BuildSelectedFields(@in.UserId),
                Filter = new DbCombinedFilter(DbLogicalOperator.And)
                {
                    Filters = new(
                        DbIncidentCustomer.CustomerIdNotNullFilter,
                        DbIncidentCustomer.BuildDateFilter(@in.MinCreationTime))
                },
                Orders = DbIncidentCustomer.DefaultOrders
            })
        .PipeValue(
            sqlApi.QueryEntitySetOrFailureAsync<DbIncidentCustomer>)
        .Map(
            static success => new LastCustomerSetGetOut
            {
                Customers = success.Map(MapCustomer)
            },
            static failure => failure.MapFailureCode(AsUnknownFailureCode));

    private static CustomerItemOut MapCustomer(DbIncidentCustomer dbIncidentCustomer)
        =>
        new(
            id: dbIncidentCustomer.CustomerId,
            title: dbIncidentCustomer.CustomerName);

    private static CustomerSetGetFailureCode AsUnknownFailureCode(Unit _)
        =>
        CustomerSetGetFailureCode.Unknown;
}