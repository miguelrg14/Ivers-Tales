using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class VolumeMathTests
    {
        [Theory]
        [InlineData(1f, 0f)]
        [InlineData(0.1f, -20f)]
        [InlineData(0.01f, -40f)]
        public void LinearToDecibels_UsesTwentyLog10(float linear, float expectedDb)
        {
            Assert.Equal(expectedDb, VolumeMath.LinearToDecibels(linear), 3);
        }

        [Theory]
        [InlineData(0f)]
        [InlineData(-1f)]
        [InlineData(float.NaN)]
        public void LinearToDecibels_SilenceIsMinusEighty_NotInfinity(float linear)
        {
            // Mathf.Log10(0) * 20 is -infinity, which the mixer rejects.
            Assert.Equal(VolumeMath.MinDecibels, VolumeMath.LinearToDecibels(linear));
        }

        [Fact]
        public void LinearToDecibels_IsClampedToMixerRange()
        {
            Assert.Equal(VolumeMath.MaxDecibels, VolumeMath.LinearToDecibels(1000f));
        }

        [Theory]
        [InlineData(1f)]
        [InlineData(0.5f)]
        [InlineData(0.25f)]
        public void DecibelsToLinear_IsInverseOfLinearToDecibels(float linear)
        {
            float db = VolumeMath.LinearToDecibels(linear);
            Assert.Equal(linear, VolumeMath.DecibelsToLinear(db), 4);
        }

        [Fact]
        public void DecibelsToLinear_MinusEightyIsSilence()
        {
            Assert.Equal(0f, VolumeMath.DecibelsToLinear(-80f));
        }

        [Theory]
        [InlineData(0.75f, 1f, "75%")]
        [InlineData(1f, 1f, "100%")]
        [InlineData(0f, 1f, "0%")]
        [InlineData(5f, 10f, "50%")]
        [InlineData(2f, 1f, "100%")]
        [InlineData(0.5f, 0f, "0%")]
        public void ToPercentLabel_FormatsSliderValue(float value, float max, string expected)
        {
            Assert.Equal(expected, VolumeMath.ToPercentLabel(value, max));
        }
    }
}
