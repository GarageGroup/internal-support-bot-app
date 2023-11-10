using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentOwner
{
    public static FlatArray<string> AllSelectedFields
        =>
        QueryAll.SelectedFields.Concat(
            $"MAX({AliasName}.createdon) AS {MaxCreatedOnParameter}");

    [DbSelect(All, AliasName, OwnerIdFieldName, GroupBy = true)]
    public Guid OwnerId { get; init; }

    [DbSelect(All, UserAlias, $"{UserAlias}.fullname", GroupBy = true)]
    public string? OwnerName { get; init; }
}