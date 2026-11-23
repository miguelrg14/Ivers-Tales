using System;

namespace IversTales.Core
{
    /// <summary>
    /// Conversions between a linear slider value (0..1) and the decibel value an
    /// AudioMixer expects. Unity mixers go from -80 dB (silence) to +20 dB.
    /// </summary>
    public static class VolumeMath
    {
        public const float MinDecibels = -80f;
        public const float MaxDecibels = 20f;

        /// <summary>Smallest linear value that is not treated as silence (-80 dB).</summary>
        public const float MinLinear = 0.0001f;

        public static float LinearToDecibels(float linear)
        {
            if (float.IsNaN(linear) || linear <= MinLinear)
                return MinDecibels;
            float db = 20f * (float)Math.Log10(linear);
            return Math.Min(MaxDecibels, Math.Max(MinDecibels, db));
        }

        public static float DecibelsToLinear(float decibels)
        {
            if (float.IsNaN(decibels) || decibels <= MinDecibels)
                return 0f;
            return (float)Math.Pow(10.0, Math.Min(decibels, MaxDecibels) / 20.0);
        }

        /// <summary>Text shown next to a volume slider, e.g. "75%".</summary>
        public static string ToPercentLabel(float value, float maxValue)
        {
            if (maxValue <= 0f) return "0%";
            float ratio = Math.Min(1f, Math.Max(0f, value / maxValue));
            return (int)Math.Round(ratio * 100f, MidpointRounding.AwayFromZero) + "%";
        }
    }
}
