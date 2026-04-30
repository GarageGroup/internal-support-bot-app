using System;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmProject.Test;

partial class CrmProjectApiTestSource
{
    public static TheoryData<FlatArray<DbProject>, ProjectSetGetOut> OutputGetTestData
        =>
        new()
        {
            {
                default,
                default
            },
            {
                [
                    new()
                    {
                        ProjectId = new("d8e5b846-dcec-4f76-bd15-53a8dbfb87ad"),
                        ProjectName = "First project"
                    },
                    new()
                    {
                        ProjectId = new("eb7322cb-e1aa-485a-8ac4-aee3c29c6172"),
                        ProjectName = null
                    },
                    new()
                    {
                        ProjectId = new("0dba0d3f-f01a-49a2-893d-16f96c072a1b"),
                        ProjectName = "Second project"
                    }
                ],
                new()
                {
                    Projects =
                    [
                        new(new("d8e5b846-dcec-4f76-bd15-53a8dbfb87ad"), "First project"),
                        new(new("eb7322cb-e1aa-485a-8ac4-aee3c29c6172"), string.Empty),
                        new(new("0dba0d3f-f01a-49a2-893d-16f96c072a1b"), "Second project")
                    ]
                }
            }
        };
}
