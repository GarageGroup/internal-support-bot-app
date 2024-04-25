using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncident
{
    [DbSelect(All, AliasName, $"{AliasName}.customerid")]
    public Guid CustomerId { get; init; }

    [DbSelect(All, AliasName, $"{AliasName}.customeridname")]
    public string? CustomerName { get; init; }

    [DbSelect(All, AliasName, $"{AliasName}.primarycontactid")]
    public Guid ContactId { get; init; }

    [DbSelect(All, AliasName, $"{AliasName}.primarycontactidname")]
    public string? ContactName { get; init; }
}