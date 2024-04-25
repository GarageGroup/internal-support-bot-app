using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class CrmIncidentApi
{
    public ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> CreateAsync(
        IncidentCreateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .Pipe(
            static @in => new IncidentJsonCreateIn
            {
                OwnerId = IncidentJsonCreateIn.BuildOwnerLookupValue(@in.OwnerId),
                CustomerId = IncidentJsonCreateIn.BuildCustomerLookupValue(@in.CustomerId),
                ContactId = @in.ContactId is null ? null : IncidentJsonCreateIn.BuildContactLookupValue(@in.ContactId.Value),
                Title = @in.Title,
                Description = @in.Description,
                CaseTypeCode = @in.CaseTypeCode switch
                {
                    IncidentCaseTypeCode.Question   => 1,
                    IncidentCaseTypeCode.Problem    => 2,
                    IncidentCaseTypeCode.Request    => 3,
                    _ => null
                },
                PriorityCode = @in.PriorityCode switch
                {
                    IncidentPriorityCode.Hight  => 1,
                    IncidentPriorityCode.Normal => 2,
                    IncidentPriorityCode.Low    => 3,
                    _ => null
                },
                SenderTelegramId = @in.SenderTelegramId.OrNullIfWhiteSpace()
            })
        .Pipe(
            IncidentJsonCreateIn.BuildDataverseCreateInput)
        .PipeValue(
            dataverseApi.Impersonate(input.CallerUserId).CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>)
        .MapFailure(
            static failure => failure.MapFailureCode(ToIncidentCreateFailureCode))
        .MapSuccess(
            static entityCreateOut => new IncidentCreateOut(
                id: entityCreateOut.Value.IncidentId,
                title: entityCreateOut.Value.Title));

    private static IncidentCreateFailureCode ToIncidentCreateFailureCode(DataverseFailureCode dataverseFailureCode)
        =>
        dataverseFailureCode switch
        {
            DataverseFailureCode.RecordNotFound     => IncidentCreateFailureCode.NotFound,
            DataverseFailureCode.UserNotEnabled     => IncidentCreateFailureCode.NotAllowed,
            DataverseFailureCode.PrivilegeDenied    => IncidentCreateFailureCode.NotAllowed,
            DataverseFailureCode.Throttling         => IncidentCreateFailureCode.TooManyRequests,
            _ => default
        };
}