using GarageGroup.Infra;
using System;

namespace GarageGroup.Internal.Support;

partial record class DbProject
{
    [DbSelect(All, AliasName, $"{AliasName}.gg_projectid")]
    public Guid ProjectId { get; init; }

    [DbSelect(All, AliasName, $"{AliasName}.gg_name")]
    public string? ProjectName { get; init; }
}