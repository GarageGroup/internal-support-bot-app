using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbProject
{
    internal static readonly DbRawFilter IsActiveFilter
        =
        new($"{AliasName}.statecode = 0");

    internal static DbParameterFilter BuildClientIdFilter(Guid clientId)
        =>
        new($"{AliasName}.gg_clientid", DbFilterOperator.Equal, clientId, "clientId");
}