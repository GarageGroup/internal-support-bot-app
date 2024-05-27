using System;
using System.Collections.Generic;
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
            input, cancellationToken)
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
                    IncidentCaseTypeCode.Question => 1,
                    IncidentCaseTypeCode.Problem => 2,
                    IncidentCaseTypeCode.Request => 3,
                    _ => null
                },
                PriorityCode = @in.PriorityCode switch
                {
                    IncidentPriorityCode.High => 1,
                    IncidentPriorityCode.Normal => 2,
                    IncidentPriorityCode.Low => 3,
                    _ => null
                },
                SenderTelegramId = @in.SenderTelegramId?.ToString()
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
                title: entityCreateOut.Value.Title))
        .MapSuccess(
            @out => new AnnotationSetInput(input.Pictures, input.CallerUserId, @out))
        .MapSuccessValue(
            CreateAnnotationsAsync);

    private ValueTask<IncidentCreateOut> CreateAnnotationsAsync(
        AnnotationSetInput input, CancellationToken cancellationToken)
    {
        if (input.Pictures.IsEmpty)
        {
            return new(input.Incident);
        }

        var inputs = input.Pictures.Map(CreateAnnotationInput);
        return AsyncPipeline.Pipe(inputs, cancellationToken).PipeParallelValue(CreateAnnotationAsync).Pipe(GetIncident);

        AnnotationInput CreateAnnotationInput(PictureModel pictureModel)
            =>
            new(pictureModel, input.CallerUserId, input.Incident.Id);

        IncidentCreateOut GetIncident(FlatArray<AnnotationCreateFailure?> failures)
            =>
            input.Incident with
            {
                Failures = GetNotNull(failures).ToFlatArray()
            };

        static IEnumerable<AnnotationCreateFailure> GetNotNull(FlatArray<AnnotationCreateFailure?> failures)
        {
            foreach (var failure in failures)
            {
                if (failure is not null)
                {
                    yield return failure;
                }                
            }
        }
    }

    private ValueTask<AnnotationCreateFailure?> CreateAnnotationAsync(
        AnnotationInput input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input.Picture, cancellationToken)
        .Pipe(
            static @in => new HttpSendIn(
                method: HttpVerb.Get,
                requestUri: @in.ImageUrl))
        .PipeValue(
            httpApi.SendAsync)
        .Forward(
            MapSuccessHttpApiOrFailure,
            static failure => failure.ToStandardFailure().WithFailureCode<Unit>(default))
        .MapSuccess(
            @out => new AnnotationJsonCreateIn(input.IncidentId, @out, input.Picture.FileName)
            {
                Subject = PictureSubject
            })
        .MapSuccess(
            AnnotationJsonCreateIn.BuildDataverseCreateInput)
        .ForwardValue(
            dataverseApi.Impersonate(input.CallerUserId).CreateEntityAsync,
            static failure => failure.WithFailureCode<Unit>(default))
        .Fold<AnnotationCreateFailure?>(
            _ => null,
            failure => new AnnotationCreateFailure(input.Picture.FileName, failure.FailureMessage)
            {
                SourceException = failure.SourceException
            });

    private static Result<byte[], Failure<Unit>> MapSuccessHttpApiOrFailure(HttpSendOut httpResponse)
    {
        if (httpResponse.Body.Content == null)
        {
            return Failure.Create("Http response content is empty");
        }
        
        return httpResponse.Body.Content.ToArray();
    }

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