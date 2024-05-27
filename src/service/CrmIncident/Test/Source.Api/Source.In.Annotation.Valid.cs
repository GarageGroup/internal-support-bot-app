using GarageGroup.Infra;
using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmIncident.Test;

partial class CrmIncidentApiTestSource
{
    public static TheoryData<IncidentCreateIn, IncidentJsonCreateOut, HttpSendOut, DataverseEntityCreateIn<AnnotationJsonCreateIn>> AnnotationInputValidTestData
        =>
        new()
        {
            {
                new(
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
                },
                new()
                {
                    IncidentId = new("ec8c8180-8ed7-4598-9bee-275262b396e2"),
                    Title = "Some Incident title"
                },
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = new HttpBody()
                    {
                        Content = new BinaryData("Some content")
                    }
                },
                new(
                    entityPluralName: "annotations",
                    entityData: new(new("ec8c8180-8ed7-4598-9bee-275262b396e2"), new BinaryData("Some content").ToArray().ToFlatArray(), "some file name")
                    {
                        Subject = "Picture from user"
                    })
            },
        };
}