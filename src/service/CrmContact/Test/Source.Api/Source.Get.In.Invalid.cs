using System.Text.Json;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Support.Service.CrmContact.Test;

partial class CrmContactApiTestSource
{
    public static TheoryData<ContactGetIn> InputGetInvalidTestData
        =>
        new()
        {
            {
                new(string.Empty)
            },
            {
                new(new(' ', 3))
            },
            {
                new(null!)
            },
        };
}