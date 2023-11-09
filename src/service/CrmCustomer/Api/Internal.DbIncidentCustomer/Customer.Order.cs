using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentCustomer
{
    internal static readonly FlatArray<DbOrder> DefaultOrders
        =
        new(
            new(CreatedByCurrentUserAlias, DbOrderType.Descending),
            new(CreatedOnAlias, DbOrderType.Descending));
}