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
                    imageUrls: default)
            },
            {
                new(
                    message: TestData.EmptyString,
                    imageUrls: default)
            },
            {
                new(
                    message: TestData.MixedWhiteSpacesString,
                    imageUrls: default)
            },
            {
                new(
                    message: TestData.EmptyString,
                    imageUrls: default)
            },
            {
                new(
                    message: TestData.MixedWhiteSpacesString,
                    imageUrls: default)
            }
        };
}
