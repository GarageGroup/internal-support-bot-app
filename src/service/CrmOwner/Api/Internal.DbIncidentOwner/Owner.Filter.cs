﻿using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentOwner
{
    internal static readonly DbRawFilter EnabledUserFilter
        =
        new($"{UserAlias}.isdisabled = 0");

    internal static DbParameterFilter BuildCurrentUserIdFilter(Guid currentUserId)
        =>
        new(OwnerIdFieldName, DbFilterOperator.Inequal, currentUserId, "currentUserId");

    internal static DbParameterFilter BuildCustomerIdFilter(Guid customerId)
        =>
        new($"{AliasName}.customerid", DbFilterOperator.Equal, customerId, "customerId");
}