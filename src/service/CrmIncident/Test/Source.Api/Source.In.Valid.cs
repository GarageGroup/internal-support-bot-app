using System;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmIncident.Test;

partial class CrmIncidentApiTestSource
{
    public static TheoryData<IncidentCreateIn, DataverseEntityCreateIn<IncidentJsonCreateIn>> InputValidTestData
        =>
        new()
        {
            {
                new(
                    ownerId: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
                    customerId: Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190"),
                    contactId: Guid.Parse("be761c38-5d95-47c2-b4aa-1056e61a1cb0"),
                    title: "Some title",
                    description: "Some description",
                    caseTypeCode: IncidentCaseTypeCode.Question,
                    priorityCode: IncidentPriorityCode.Low,
                    callerUserId: Guid.Parse("8d690bea-2c1d-4ded-b5c2-0d070e8559f1")),
                new(
                    entityPluralName: "incidents",
                    selectFields: new("title"),
                    entityData: new()
                    {
                        OwnerId = "/systemusers(1203c0e2-3648-4596-80dd-127fdd2610b6)",
                        CustomerId = "/accounts(bd8b8e33-554e-e611-80dc-c4346bad0190)",
                        ContactId = "/contacts(be761c38-5d95-47c2-b4aa-1056e61a1cb0)",
                        Title = "Some title",
                        Description = "Some description",
                        CaseTypeCode = 1,
                        PriorityCode = 3,
                        CaseOriginCode = null
                    })
            },
            {
                new(
                    ownerId: Guid.Parse("8de1c47f-5d1b-47d5-9a79-ec55f2f24142"),
                    customerId: Guid.Parse("04abdfa4-deae-4486-8611-450a6c538159"),
                    contactId: null,
                    title: string.Empty,
                    description: "Some Description",
                    caseTypeCode: IncidentCaseTypeCode.Request,
                    priorityCode: IncidentPriorityCode.Hight,
                    callerUserId: Guid.Parse("144326e7-5aa8-4792-b8aa-d125b6f7f6b5")),
                new(
                    entityPluralName: "incidents",
                    selectFields: new("title"),
                    entityData: new()
                    {
                        OwnerId = "/systemusers(8de1c47f-5d1b-47d5-9a79-ec55f2f24142)",
                        CustomerId = "/accounts(04abdfa4-deae-4486-8611-450a6c538159)",
                        ContactId = null,
                        Title = string.Empty,
                        Description = "Some Description",
                        CaseTypeCode = 3,
                        PriorityCode = 1,
                        CaseOriginCode = null
                    })
            },
            {
                new(
                    ownerId: Guid.Parse("0c03e968-b0e0-4e0c-b2b3-7594d73882a1"),
                    customerId: Guid.Parse("4a4b12a1-5034-420b-91bc-cb0a251c3b01"),
                    contactId: Guid.Parse("5d22feb3-b450-4129-8bfb-729043042dfa"),
                    title: "Some Title",
                    description: string.Empty,
                    caseTypeCode: IncidentCaseTypeCode.Problem,
                    priorityCode: IncidentPriorityCode.Normal,
                    callerUserId: Guid.Parse("a6093320-cdf2-43d7-bf42-21825c303721")),
                new(
                    entityPluralName: "incidents",
                    selectFields: new("title"),
                    entityData: new()
                    {
                        OwnerId = "/systemusers(0c03e968-b0e0-4e0c-b2b3-7594d73882a1)",
                        CustomerId = "/accounts(4a4b12a1-5034-420b-91bc-cb0a251c3b01)",
                        ContactId = "/contacts(5d22feb3-b450-4129-8bfb-729043042dfa)",
                        Title = "Some Title",
                        Description = string.Empty,
                        CaseTypeCode = 2,
                        PriorityCode = 2,
                        CaseOriginCode = null
                    })
            }
        };
}