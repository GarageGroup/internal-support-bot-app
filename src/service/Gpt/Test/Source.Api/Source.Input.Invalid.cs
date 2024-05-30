using PrimeFuncPack.UnitTest;
using Xunit;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

partial class SupportGptApiTestSource
{
    public static TheoryData<SupportGptApiOption, IncidentCompleteIn> InputInvalidTestData
        =>
        new()
        {
            {
                new(default)
                {
                    IsImageProcessing = true,
                },
                new(
                    message: null,
                    imageUrls: default)
            },
            {
                new(default)
                {
                    IsImageProcessing = true,
                },
                new(
                    message: TestData.EmptyString,
                    imageUrls: default)
            },
            {
                new(default)
                {
                    IsImageProcessing = true,
                },
                new(
                    message: TestData.MixedWhiteSpacesString,
                    imageUrls: default)
            },
            {
                new(default)
                {
                    IsImageProcessing = true,
                },
                new(
                    message: TestData.EmptyString,
                    imageUrls: default)
            },
            {
                new(default)
                {
                    IsImageProcessing = true,
                },
                new(
                    message: TestData.MixedWhiteSpacesString,
                    imageUrls: default)
            },
            {
                new(default)
                {
                    IsImageProcessing = false,
                },
                new(
                    message: null,
                    imageUrls: ["some image"])
            }
        };
}
