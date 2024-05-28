using System;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Support.Service.CrmIncident.Test;

public static partial class CrmIncidentApiTest
{
    private static readonly IncidentCreateIn SomeIncidentCreateInput
        =
        new(
            ownerId: new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: new("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: new("7a8b61a2-7f83-4519-9bd6-9a50c124613d"),
            title: "title",
            description: "decription",
            caseTypeCode: IncidentCaseTypeCode.Question,
            priorityCode: IncidentPriorityCode.Normal,
            callerUserId: new("d40c9c6c-ba5f-4264-aaad-542f11caf4f6"));

    private static readonly IncidentJsonCreateOut SomeIncidentJsonOutput
        =
        new()
        {
            IncidentId = new("ec8c8180-8ed7-4598-9bee-275262b396e2"),
            Title = "Some Incident title"
        };

    private static readonly HttpSendOut SomeHttpApiSuccessOutput
        =
        new()
        {
            StatusCode = HttpSuccessCode.OK,
            Body = new()
            {
                Content = new("Some content")
            }
        };

    private static Mock<IDataverseEntityCreateSupplier> BuildMockDataverseCreateSupplier(
        in Result<DataverseEntityCreateOut<IncidentJsonCreateOut>, Failure<DataverseFailureCode>> incidentResult,
        in Result<Unit, Failure<DataverseFailureCode>> annotationResult)
    {
        var mock = new Mock<IDataverseEntityCreateSupplier>();

        _ = mock.Setup(
            static api => api.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(
                It.IsAny<DataverseEntityCreateIn<IncidentJsonCreateIn>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(incidentResult);

        _ = mock.Setup(
            static api => api.CreateEntityAsync(
                It.IsAny<DataverseEntityCreateIn<AnnotationJsonCreateIn>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(annotationResult);

        return mock;
    }

    private static Mock<IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>> BuildMockDataverseApi(
        IDataverseEntityCreateSupplier dataverseCreateSupplier)
    {
        var mock = new Mock<IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>>();

        _ = mock.Setup(static api => api.Impersonate(It.IsAny<Guid>())).Returns(dataverseCreateSupplier);

        return mock;
    }

    private static Mock<IHttpApi> BuildHttpApi(in Result<HttpSendOut, HttpSendFailure> result)
    {
        var mock = new Mock<IHttpApi>();

        _ = mock.Setup(static a => a.SendAsync(It.IsAny<HttpSendIn>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        return mock;
    }
}