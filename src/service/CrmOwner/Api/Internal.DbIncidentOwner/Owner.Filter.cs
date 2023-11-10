using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentOwner
{
    internal static readonly DbRawFilter OwnerNotNullFilter
        =
        new($"{OwnerIdFieldName} IS NOT NULL");

    internal static DbParameterFilter BuildCurrentUserIdFilter(Guid currentUserId)
        =>
        new(OwnerIdFieldName, DbFilterOperator.Inequal, currentUserId, "currentUserId");

    internal static DbParameterFilter BuildCustomerIdFilter(Guid customerId)
        =>
        new($"{AliasName}.customerid", DbFilterOperator.Equal, customerId, "customerId");
}