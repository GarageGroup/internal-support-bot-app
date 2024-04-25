using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

partial class CrmContactApi
{
    public ValueTask<Result<ContactGetOut, Failure<ContactGetFailureCode>>> GetAsync(
        ContactGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            Validate)
        .MapSuccess(
            static @in => DbIncident.QueryAll with
            {
                Top = 1,
                Filter = DbIncident.BuildFilter(@in.TelegramSenderId),
                Orders = DbIncident.DefaultOrders
            })
        .ForwardValue(
            sqlApi.QueryEntitySetOrFailureAsync<DbIncident>,
            static failure => failure.WithFailureCode(ContactGetFailureCode.Unknown))
        .Forward(
            MapIncidentOrFailure);

    private Result<ContactGetIn, Failure<ContactGetFailureCode>> Validate(ContactGetIn input)
    {
        if (string.IsNullOrWhiteSpace(input.TelegramSenderId))
        {
            return Failure.Create(ContactGetFailureCode.InvalidInput, "Telegram sender id is empty");
        }

        return input;
    }

    private Result<ContactGetOut, Failure<ContactGetFailureCode>> MapIncidentOrFailure(FlatArray<DbIncident> incidentList)
    {
        if (incidentList.IsEmpty)
        {
            return Failure.Create(ContactGetFailureCode.NotFound, "Incident not found");
        }

        var incident = incidentList[0];

        return new ContactGetOut(
            contactId: incident.ContactId,
            contactName: incident.ContactName,
            customerId: incident.CustomerId,
            customerName: incident.CustomerName);
    }
}