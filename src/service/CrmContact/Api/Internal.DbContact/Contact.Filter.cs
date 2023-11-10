using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbContact
{
    internal static DbParameterFilter BuildCustomerIdFilter(Guid customerId)
        =>
        new($"{AliasName}.parentcustomerid", DbFilterOperator.Equal, customerId, "customerId");
}