using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
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
        var dataverseIncidentCreateOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseIncidentCreateOut, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);
        var mockHttpApi = BuildHttpApi(SomeHttpApiSuccessOutput);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

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
    [MemberData(nameof(CrmIncidentApiTestSource.IncidentInputValidTestData), MemberType = typeof(CrmIncidentApiTestSource))]
    internal static async Task CreateAsync_InputIsNotNull_ExpectDataverseCreateCalledOnce(
        IncidentCreateIn input, DataverseEntityCreateIn<IncidentJsonCreateIn> expectedInput)
    {
        var dataverseIncidentCreateOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseIncidentCreateOut, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);
        var mockHttpApi = BuildHttpApi(SomeHttpApiSuccessOutput);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

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

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseFailure, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);
        var mockHttpApi = BuildHttpApi(SomeHttpApiSuccessOutput);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

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

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseOut, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);
        var mockHttpApi = BuildHttpApi(SomeHttpApiSuccessOutput);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

        var actual = await api.CreateAsync(SomeIncidentCreateInput, default);

        var expected = new IncidentCreateOut(
            id: new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            title: "Some incident title");

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task CreateAsync_InputIsNotNullPicturesIsNotEmpty_ExpectCallHttpApiOnce()
    {
        var dataverseIncidentCreateOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseIncidentCreateOut, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);
        var mockHttpApi = BuildHttpApi(SomeHttpApiSuccessOutput);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

        var input = new IncidentCreateIn(
            ownerId: new("0c1040cc-6dff-4eda-b40b-38b04b72bb82"),
            customerId: new("4b4d6147-da68-4dea-b8da-f0090d118b12"),
            contactId: new("6cdae1fe-f0c8-4664-9c0c-579aae1ce242"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.High,
            callerUserId: new("de42801c-ae9b-4be1-bd39-a0a70324539f"))
        {
            Pictures = 
            [
                new PictureModel("first some file name", "first some image url"),
                new PictureModel("second some file name", "second some image url")
            ]
        };

        _ = await api.CreateAsync(input, default);

        mockHttpApi.Verify(a => a.SendAsync(new HttpSendIn(HttpVerb.Get, "first some image url"), It.IsAny<CancellationToken>()), Times.Once);
        mockHttpApi.Verify(a => a.SendAsync(new HttpSendIn(HttpVerb.Get, "second some image url"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task CreateAsync_HttpApiIsNotSuccess_ExpectOutputFailure()
    {
        var incidentCreateOut = new IncidentJsonCreateOut()
        {
            IncidentId = new("ec8c8180-8ed7-4598-9bee-275262b396e2"),
            Title = "Some Incident title"
        };
        var dataverseIncidentCreateOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(incidentCreateOut);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseIncidentCreateOut, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);

        var httpFailure = new HttpSendFailure()
        {
            StatusCode = HttpFailureCode.BadRequest,
            Body = new()
            {
                Type = new(MediaTypeNames.Application.Json),
                Content = BinaryData.FromString("Some failure message")
            }
        };
        var mockHttpApi = BuildHttpApi(httpFailure);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

        var input = new IncidentCreateIn(
            ownerId: new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: new("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: new("be761c38-5d95-47c2-b4aa-1056e61a1cb0"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Question,
            priorityCode: IncidentPriorityCode.Low,
            callerUserId: new("8d690bea-2c1d-4ded-b5c2-0d070e8559f1"))
        {
            Pictures =
            [
                new PictureModel("some file name", "some image url")
            ]
        };

        var actual = await api.CreateAsync(input, default);
        var expected = new IncidentCreateOut(
            id: new("ec8c8180-8ed7-4598-9bee-275262b396e2"),
            title: "Some Incident title")
        {
            Failures = [new AnnotationCreateFailure("some file name", "An unexpected http failure occured: 400.\nSome failure message")]
        };

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CrmIncidentApiTestSource.AnnotationInputValidTestData), MemberType = typeof(CrmIncidentApiTestSource))]
    internal static async Task CreateAsync_InputIsNotNullPicturesIsNotEmpty_ExpectDataverseAnnotationCreateCalledOnce(
        IncidentCreateIn input,
        IncidentJsonCreateOut incidentJsonCreateOut,
        HttpSendOut httpApiOut,
        DataverseEntityCreateIn<AnnotationJsonCreateIn> expectedInput)
    {
        var dataverseIncidentCreateOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(incidentJsonCreateOut);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseIncidentCreateOut, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);
        var mockHttpApi = BuildHttpApi(httpApiOut);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

        _ = await api.CreateAsync(input, default);

        mockDataverseCreateSupplier.Verify(a => a.CreateEntityAsync(expectedInput, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange)]
    [InlineData(DataverseFailureCode.DuplicateRecord)]
    [InlineData(DataverseFailureCode.RecordNotFound)]
    [InlineData(DataverseFailureCode.PrivilegeDenied)]
    [InlineData(DataverseFailureCode.UserNotEnabled)]
    [InlineData(DataverseFailureCode.Throttling)]
    public static async Task CreateAsync_DataverseAnnotationCreateResultIsNotSuccess_ExpectOutputFailure(
        DataverseFailureCode sourceFailureCode)
    {
        var incidentCreateOut = new IncidentJsonCreateOut()
        {
            IncidentId = new("ec8c8180-8ed7-4598-9bee-275262b396e2"),
            Title = "Some Incident title"
        };
        var dataverseIncidentCreateOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(incidentCreateOut);
        
        var sourceException = new Exception("Some error message");
        var dataverseAnnotationCreateFailure = Failure.Create(sourceFailureCode, "Some failure message", sourceException);

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseIncidentCreateOut, dataverseAnnotationCreateFailure);
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);

        var mockHttpApi = BuildHttpApi(SomeHttpApiSuccessOutput);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

        var input = new IncidentCreateIn(
            ownerId: new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: new("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: new("be761c38-5d95-47c2-b4aa-1056e61a1cb0"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Question,
            priorityCode: IncidentPriorityCode.Low,
            callerUserId: new("8d690bea-2c1d-4ded-b5c2-0d070e8559f1"))
        {
            Pictures =
            [
                new PictureModel("some file name", "some image url")
            ]
        };

        var actual = await api.CreateAsync(input, default);
        var expected = new IncidentCreateOut(
            id: new("ec8c8180-8ed7-4598-9bee-275262b396e2"),
            title: "Some Incident title")
        {
            Failures = 
            [
                new("some file name", "Some failure message") 
                {
                    SourceException = sourceException
                }
            ]
        };

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task CreateAsync_DataverseResultIsSuccessPicturesIsNotEmpty_ExpectSuccess()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(
            value: new()
            {
                IncidentId = new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
                Title = "Some incident title"
            });

        var mockDataverseCreateSupplier = BuildMockDataverseCreateSupplier(dataverseOut, Result.Success<Unit>(default));
        var mockDataverseApi = BuildMockDataverseApi(mockDataverseCreateSupplier.Object);
        var mockHttpApi = BuildHttpApi(SomeHttpApiSuccessOutput);

        var api = new CrmIncidentApi(mockHttpApi.Object, mockDataverseApi.Object);

        var input = new IncidentCreateIn(
            ownerId: new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: new("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: new("be761c38-5d95-47c2-b4aa-1056e61a1cb0"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Question,
            priorityCode: IncidentPriorityCode.Low,
            callerUserId: new("8d690bea-2c1d-4ded-b5c2-0d070e8559f1"))
        {
            Pictures =
            [
                new PictureModel("some file name", "some image url")
            ]
        };
        var actual = await api.CreateAsync(input, default);

        var expected = new IncidentCreateOut(
            id: new("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            title: "Some incident title");

        Assert.StrictEqual(expected, actual);
    }
}