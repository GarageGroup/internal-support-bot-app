using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentCustomer
{
    internal static readonly FlatArray<DbOrder> DefaultOrders
        =
        [
            new(MaxCurrentUserCreatedOnParameter, DbOrderType.Descending),
            new(MaxCreatedOnParameter, DbOrderType.Descending)
        ];
}