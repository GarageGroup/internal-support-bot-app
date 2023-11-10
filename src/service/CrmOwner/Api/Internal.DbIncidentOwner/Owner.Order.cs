using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentOwner
{
    internal static readonly FlatArray<DbOrder> DefaultOrders
        =
        new DbOrder(MaxCreatedOnParameter, DbOrderType.Descending).AsFlatArray();
}