using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncident
{
    internal static readonly FlatArray<DbOrder> DefaultOrders
        =
        [
            new($"{AliasName}.createdon", DbOrderType.Descending)
        ];
}