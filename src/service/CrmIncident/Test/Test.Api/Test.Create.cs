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
    public static async Task CreateAsync_InputIsNotNull_ExpectCallImpersonateOnce()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseOut);
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);

        var api = new CrmIncidentApi(mockDataverseApi.Object);

        var input = new IncidentCreateIn(
            ownerId: new("0c1040cc-6dff-4eda-b40b-38b04b72bb82"),
            customerId: new("4b4d6147-da68-4dea-b8da-f0090d118b12"),
            contactId: new("6cdae1fe-f0c8-4664-9c0c-579aae1ce242"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.High,
            callerUserId: new("de42801c-ae9b-4be1-bd39-a0a70324539f"));

        _ = await api.CreateAsync(input, default);

        mockDataverseApi.Verify(static a => a.Impersonate(new("de42801c-ae9b-4be1-bd39-a0a70324539f")), Times.Once);
    }

    [Theory]
    [MemberData(nameof(CrmIncidentApiTestSource.InputValidTestData), MemberType = typeof(CrmIncidentApiTestSource))]
    internal static async Task CreateAsync_InputIsNotNull_ExpectDataverseCreateCalledOnce(
        IncidentCreateIn input, DataverseEntityCreateIn<IncidentJsonCreateIn> expectedInput)
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseOut);
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);

        var api = new CrmIncidentApi(mockDataverseApi.Object);

        var token = new CancellationToken(false);
        _ = await api.CreateAsync(input, token);

        mockDataverseCreateSupplier.Verify(
            a => a.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(expectedInput, token), Times.Once);
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
        var sourceException = new Exception("Some error message");
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some failure message", sourceException);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseFailure);
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);

        var api = new CrmIncidentApi(mockDataverseApi.Object);

        var actual = await api.CreateAsync(SomeIncidentCreateInput, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some failure message", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task CreateAsync_DataverseResultIsSuccess_ExpectSuccess()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(
            value: new()
            {
                IncidentId = new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
                Title = "Some incident title"
            });

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseOut);
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);

        var api = new CrmIncidentApi(mockDataverseApi.Object);

        var actual = await api.CreateAsync(SomeIncidentCreateInput, default);

        var expected = new IncidentCreateOut(
            id: new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            title: "Some incident title");

        Assert.StrictEqual(expected, actual);
    }
}