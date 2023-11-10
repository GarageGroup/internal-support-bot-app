using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentCustomer
{
    internal static readonly FlatArray<DbOrder> DefaultOrders
        =
        new(
            new(MaxCurrentUserCreatedOnAlias, DbOrderType.Descending),
            new(MaxCreatedOnAlias, DbOrderType.Descending));
}