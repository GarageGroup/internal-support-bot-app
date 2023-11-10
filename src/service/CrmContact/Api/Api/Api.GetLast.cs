using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmContactApi
{
    public ValueTask<Result<LastContactSetGetOut, Failure<ContactSetGetFailureCode>>> GetLastAsync(
        LastContactSetGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe<IDbQuery>(
            static @in => DbContact.QueryAll with
            {
                Top = @in.Top,
                SelectedFields = DbContact.BuildSelectedFields(@in.UserId),
                Filter = DbContact.BuildCustomerIdFilter(@in.CustomerId),
                Orders = DbContact.DefaultOrders
            })
        .PipeValue(
            sqlApi.QueryEntitySetOrFailureAsync<DbContact>)
        .Map(
            static success => new LastContactSetGetOut
            {
                Contacts = success.Map(MapContact)
            },
            static failure => failure.MapFailureCode(AsUnknownFailureCode));

    private static ContactItemOut MapContact(DbContact dbContact)
        =>
        new(
            id: dbContact.Id,
            fullName: dbContact.Name);

    private static ContactSetGetFailureCode AsUnknownFailureCode(Unit _)
        =>
        ContactSetGetFailureCode.Unknown;
}