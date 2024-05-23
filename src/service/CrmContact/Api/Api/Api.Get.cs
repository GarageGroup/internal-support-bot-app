using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmContactApi
{
    public ValueTask<Result<ContactGetOut, Failure<ContactGetFailureCode>>> GetAsync(
        ContactGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            static @in => DbIncident.QueryAll with
            {
                Top = 1,
                Filter = DbIncident.BuildFilter(@in.TelegramSenderId),
                Orders = DbIncident.DefaultOrders
            })
        .PipeValue(
            sqlEntityApi.QueryEntityOrFailureAsync<DbIncident>)
        .Map(
            static incident => new ContactGetOut(
                contactId: incident.ContactId,
                contactName: incident.ContactName,
                customerId: incident.CustomerId,
                customerName: incident.CustomerName),
            static failure => failure.MapFailureCode(MapFailureCode));

    private static ContactGetFailureCode MapFailureCode(EntityQueryFailureCode failureCode)
        =>
        failureCode switch
        {
            EntityQueryFailureCode.NotFound => ContactGetFailureCode.NotFound,
            _ => default
        };
}