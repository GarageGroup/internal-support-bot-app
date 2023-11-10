using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbContact
{
    public static FlatArray<string> BuildSelectedFields(Guid userId)
        =>
        QueryAll.SelectedFields.Concat(
            $"({LastIncidentDateQuery} AND {IncidentAlias}.createdby = '{userId:D}') AS {LastCurrentUserIncidentDateParameter}",
            $"({LastIncidentDateQuery}) AS {LastIncidentDateParameter}");

    [DbSelect(All, AliasName, IdFieldName)]
    public Guid Id { get; init; }

    [DbSelect(All, AliasName, $"{AliasName}.fullname")]
    public string? Name { get; init; }
}