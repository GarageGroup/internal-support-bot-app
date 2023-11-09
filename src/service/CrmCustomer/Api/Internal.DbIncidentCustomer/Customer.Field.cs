﻿using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentCustomer
{
    public static FlatArray<string> BuildSelectedFields(Guid userId)
        =>
        QueryAll.SelectedFields.Concat(
            $"MAX({CreatedOnFieldName}) AS {CreatedOnAlias}",
            $"Max(CASE WHEN {AliasName}.createdby = '{userId:D}' THEN 1 ELSE 0 END) AS {CreatedByCurrentUserAlias}");

    [DbSelect(All, AliasName, $"{CustomerIdFieldName}", GroupBy = true)]
    public Guid CustomerId { get; init; }

    [DbSelect(All, AccountAlias, $"{AccountAlias}.name", GroupBy = true)]
    public string? CustomerName { get; init; }
}