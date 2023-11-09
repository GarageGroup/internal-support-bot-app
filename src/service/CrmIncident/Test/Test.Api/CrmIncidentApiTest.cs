using System;
using System.Runtime.CompilerServices;
using System.Threading;
using GarageGroup.Infra;
using Moq;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace GarageGroup.Internal.Support.Service.CrmIncident.Test;

public static partial class CrmIncidentApiTest
{
    private static readonly IncidentCreateIn SomeIncidentCreateInput
        =
        new(
            ownerId: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: Guid.Parse("7a8b61a2-7f83-4519-9bd6-9a50c124613d"),
            title: "title",
            description: "decription",
            caseTypeCode: IncidentCaseTypeCode.Question,
            priorityCode: IncidentPriorityCode.Normal);

    private static readonly IncidentJsonCreateOut SomeIncidentJsonOutput
        =
        new()
        {
            IncidentId = Guid.Parse("ec8c8180-8ed7-4598-9bee-275262b396e2"),
            Title = "Some Incident title"
        };

    private static Mock<IStubDataverseApi> CreateMockDataverseApi(
        Result<DataverseEntityCreateOut<IncidentJsonCreateOut>, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IStubDataverseApi>();

        _ = mock.Setup(
            static api => api.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(
                It.IsAny<DataverseEntityCreateIn<IncidentJsonCreateIn>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        _ = mock.Setup(static api => api.Impersonate(It.IsAny<Guid>())).Returns(mock.Object);

        return mock;
    }

    internal interface IStubDataverseApi : IDataverseEntityCreateSupplier, IDataverseImpersonateSupplier<IDataverseEntityCreateSupplier>
    {
    }
}