using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbContact
{
    internal static readonly FlatArray<DbOrder> DefaultOrders
        =
        [
            new(LastCurrentUserIncidentDateParameter, DbOrderType.Descending),
            new(LastIncidentDateParameter, DbOrderType.Descending),
            new($"{AliasName}.createdon", DbOrderType.Descending)
        ];
}