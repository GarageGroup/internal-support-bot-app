using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial record class DbIncidentCustomer
{
    internal static DbParameterFilter BuildDateFilter(DateTime minDate)
        =>
        new(CreatedOnFieldName, DbFilterOperator.GreaterOrEqual, minDate, "minDate");
}