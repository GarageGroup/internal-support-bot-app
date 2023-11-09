using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmIncident.Test;

partial class CrmIncidentApiTest
{
    [Fact]
    public static async Task CreateAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmIncidentApi<IStubDataverseApi>(mockDataverseApi.Object);

        var cancellationToken = new CancellationToken(canceled: false);
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);

        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.CreateAsync(null!, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(CrmIncidentApiTestSource.InputValidTestData), MemberType = typeof(CrmIncidentApiTestSource))]
    internal static async Task CreateAsync_InputIsNotNull_ExpectCallDataverseApiClientOnce(
        IncidentCreateIn input, DataverseEntityCreateIn<IncidentJsonCreateIn> expectedInput)
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmIncidentApi<IStubDataverseApi>(mockDataverseApi.Object);

        var token = new CancellationToken(false);
        _ = await api.CreateAsync(input, token);

        mockDataverseApi.Verify(a => a.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(expectedInput, token), Times.Once);
    }

    [Fact]
    public static async Task CreateAsync_CallerUserIdIsNull_ExpectCallImpersonateNever()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmIncidentApi<IStubDataverseApi>(mockDataverseApi.Object);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse("041eb1fd-c185-4e17-9ce3-7bb754ce84b6"),
            customerId: Guid.Parse("b3a2b17c-3c49-4c58-b365-3ff6dc168b6d"),
            contactId: Guid.Parse("3340639e-847c-49e0-9bad-ee05a8ea0a0f"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.Hight);

        _ = await api.CreateAsync(input, default);
        mockDataverseApi.Verify(static a => a.Impersonate(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public static async Task CreateAsync_CallerUserIdIsNotNull_ExpectCallImpersonateOnce()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmIncidentApi<IStubDataverseApi>(mockDataverseApi.Object);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse("0c1040cc-6dff-4eda-b40b-38b04b72bb82"),
            customerId: Guid.Parse("4b4d6147-da68-4dea-b8da-f0090d118b12"),
            contactId: Guid.Parse("6cdae1fe-f0c8-4664-9c0c-579aae1ce242"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.Hight)
        {
            CallerUserId = Guid.Parse("de42801c-ae9b-4be1-bd39-a0a70324539f")
        };

        _ = await api.CreateAsync(input, default);

        mockDataverseApi.Verify(static a => a.Impersonate(Guid.Parse("de42801c-ae9b-4be1-bd39-a0a70324539f")), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Unknown, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, IncidentCreateFailureCode.NotFound)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, IncidentCreateFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.UserNotEnabled, IncidentCreateFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.Throttling, IncidentCreateFailureCode.TooManyRequests)]
    public static async Task CreateAsync_DataverseResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, IncidentCreateFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some failure message");
        var mockDataverseApi = CreateMockDataverseApi(dataverseFailure);

        var api = new CrmIncidentApi<IStubDataverseApi>(mockDataverseApi.Object);

        var actual = await api.CreateAsync(SomeIncidentCreateInput, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, dataverseFailure.FailureMessage);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task CreateAsync_DataverseResultIsSuccess_ExpectSuccess()
    {
        var incidentJsonOut = new IncidentJsonCreateOut
        {
            IncidentId = Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            Title = "Some incident title"
        };

        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(incidentJsonOut);
        var mockDataverseApi = CreateMockDataverseApi(dataverseOut);

        var api = new CrmIncidentApi<IStubDataverseApi>(mockDataverseApi.Object);

        var actual = await api.CreateAsync(SomeIncidentCreateInput, default);

        var expected = new IncidentCreateOut(
            id: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            title: "Some incident title");

        Assert.StrictEqual(expected, actual);
    }
}