using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmOwnerApi
{
    public ValueTask<Result<LastOwnerSetGetOut, Failure<OwnerSetGetFailureCode>>> GetLastAsync(
        LastOwnerSetGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe<IDbQuery>(
            static @in => DbIncidentOwner.QueryAll with
            {
                Top = @in.Top,
                SelectedFields = DbIncidentOwner.AllSelectedFields,
                Filter = new DbCombinedFilter(DbLogicalOperator.And)
                {
                    Filters = new(
                        DbIncidentOwner.BuildCurrentUserIdFilter(@in.UserId),
                        DbIncidentOwner.BuildCustomerIdFilter(@in.CustomerId),
                        DbIncidentOwner.EnabledUserFilter)
                },
                Orders = DbIncidentOwner.DefaultOrders
            })
        .PipeValue(
            sqlApi.QueryEntitySetOrFailureAsync<DbIncidentOwner>)
        .Map(
            static success => new LastOwnerSetGetOut
            {
                Owners = success.Map(MapOwner)
            },
            static failure => failure.WithFailureCode(OwnerSetGetFailureCode.Unknown));

    private static OwnerItemOut MapOwner(DbIncidentOwner dbIncidentOwner)
        =>
        new(
            id: dbIncidentOwner.OwnerId,
            fullName: dbIncidentOwner.OwnerName);
}