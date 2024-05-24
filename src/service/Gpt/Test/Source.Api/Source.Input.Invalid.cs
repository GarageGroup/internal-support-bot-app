using GarageGroup.Infra;
using PrimeFuncPack.UnitTest;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<IncidentCompleteIn> InputInvalidTestData
        =>
        new()
        {
            {
                new(
                    message: null,
                    imageUrl: default)
            },
            {
                new(
                    message: TestData.EmptyString,
                    imageUrl: default)
            },
            {
                new(
                    message: TestData.MixedWhiteSpacesString,
                    imageUrl: default)
            },
            {
                new(
                    message: TestData.EmptyString,
                    imageUrl: default)
            },
            {
                new(
                    message: TestData.MixedWhiteSpacesString,
                    imageUrl: default)
            }
        };
}
