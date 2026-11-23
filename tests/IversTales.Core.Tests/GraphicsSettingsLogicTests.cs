using System.Collections.Generic;
using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class GraphicsSettingsLogicTests
    {
        [Fact]
        public void DistinctResolutions_RemovesRefreshRateDuplicatesAndSorts()
        {
            var raw = new[]
            {
                new ScreenSize(1920, 1080), new ScreenSize(1280, 720),
                new ScreenSize(1920, 1080), new ScreenSize(1280, 720),
                new ScreenSize(2560, 1440), new ScreenSize(0, 0),
            };

            var result = GraphicsSettingsLogic.DistinctResolutions(raw);

            Assert.Equal(new[] { new ScreenSize(1280, 720), new ScreenSize(1920, 1080), new ScreenSize(2560, 1440) }, result);
        }

        [Fact]
        public void DistinctResolutions_HandlesNull()
        {
            Assert.Empty(GraphicsSettingsLogic.DistinctResolutions(null));
        }

        [Fact]
        public void IndexOfResolution_FindsCurrent()
        {
            var list = new List<ScreenSize> { new ScreenSize(1280, 720), new ScreenSize(1920, 1080) };
            Assert.Equal(1, GraphicsSettingsLogic.IndexOfResolution(list, new ScreenSize(1920, 1080)));
        }

        [Fact]
        public void IndexOfResolution_FallsBackToLargest_InsteadOfOutOfRange()
        {
            // Regression: the default index was resolutions.Length, one past the end.
            var list = new List<ScreenSize> { new ScreenSize(1280, 720), new ScreenSize(1920, 1080) };
            Assert.Equal(1, GraphicsSettingsLogic.IndexOfResolution(list, new ScreenSize(800, 600)));
            Assert.Equal(-1, GraphicsSettingsLogic.IndexOfResolution(new List<ScreenSize>(), new ScreenSize(800, 600)));
        }

        [Theory]
        [InlineData("Low", 0, 1)]
        [InlineData("Medium", 1, 2)]
        [InlineData("high", 2, 3)]
        [InlineData("Unknown", 1, 1)]
        [InlineData("Unknown", 99, 5)]
        public void ResolveQualityLevel_MatchesByName(string label, int optionIndex, int expected)
        {
            var names = new[] { "Very Low", "Low", "Medium", "High", "Very High", "Ultra" };
            Assert.Equal(expected, GraphicsSettingsLogic.ResolveQualityLevel(names, label, optionIndex));
        }

        [Theory]
        [InlineData(0.9f, 0f)]
        [InlineData(0f, 0.9f)]
        [InlineData(0.4f, 0.5f)]
        [InlineData(5f, 0f)]
        public void OverlayAlpha_IsInverseOfBrightness(float brightness, float expectedAlpha)
        {
            Assert.Equal(expectedAlpha, GraphicsSettingsLogic.OverlayAlphaForBrightness(brightness, 0f, 0.9f), 4);
        }

        [Theory]
        [InlineData(26f, 90f, 26f)]
        [InlineData(26f, 60f, 17.3333f)]
        [InlineData(30f, 120f, 40f)]
        public void ScaleFieldOfView_KeepsAuthoredFovAtDefault(float authored, float slider, float expected)
        {
            Assert.Equal(expected, GraphicsSettingsLogic.ScaleFieldOfView(authored, slider), 3);
        }

        [Fact]
        public void ScaleFieldOfView_IgnoresInvalidSlider()
        {
            Assert.Equal(26f, GraphicsSettingsLogic.ScaleFieldOfView(26f, 0f));
        }

        [Theory]
        [InlineData(10, 30)]
        [InlineData(144, 144)]
        [InlineData(1000, 240)]
        [InlineData(0, -1)]
        public void ClampFrameRate(int requested, int expected)
        {
            Assert.Equal(expected, GraphicsSettingsLogic.ClampFrameRate(requested));
        }
    }
}
