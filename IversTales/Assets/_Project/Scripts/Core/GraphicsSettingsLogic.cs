using System;
using System.Collections.Generic;
using System.Linq;

namespace IversTales.Core
{
    /// <summary>Plain resolution value, independent from UnityEngine.Resolution.</summary>
    public readonly struct ScreenSize : IEquatable<ScreenSize>
    {
        public readonly int Width;
        public readonly int Height;

        public ScreenSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(ScreenSize other) => Width == other.Width && Height == other.Height;
        public override bool Equals(object obj) => obj is ScreenSize other && Equals(other);
        public override int GetHashCode() => (Width * 397) ^ Height;
        public override string ToString() => Width + " x " + Height;
    }

    /// <summary>
    /// Pure logic behind the graphics tab of the options menu.
    /// </summary>
    public static class GraphicsSettingsLogic
    {
        public const string ResolutionKey = "resolution";
        public const string QualityKey = "quality";
        public const string ScreenModeKey = "screenMode";
        public const string BrightnessKey = "brightness";
        public const string FieldOfViewKey = "fov";
        public const string FpsLimitKey = "fpsLimit";
        public const string VSyncKey = "vSync";

        /// <summary>
        /// Screen.resolutions returns one entry per refresh rate, so the same size
        /// appears several times. Keeps each size once, ordered from smallest to largest.
        /// </summary>
        public static List<ScreenSize> DistinctResolutions(IEnumerable<ScreenSize> resolutions)
        {
            if (resolutions == null) return new List<ScreenSize>();
            return resolutions
                .Where(r => r.Width > 0 && r.Height > 0)
                .Distinct()
                .OrderBy(r => r.Width)
                .ThenBy(r => r.Height)
                .ToList();
        }

        /// <summary>Index of <paramref name="current"/> in the list, or the largest resolution if it is not listed.</summary>
        public static int IndexOfResolution(IReadOnlyList<ScreenSize> resolutions, ScreenSize current)
        {
            if (resolutions == null || resolutions.Count == 0) return -1;
            for (int i = 0; i < resolutions.Count; i++)
                if (resolutions[i].Equals(current))
                    return i;
            return resolutions.Count - 1;
        }

        /// <summary>
        /// Maps a dropdown label ("Low", "Medium", "High") to the index of the quality
        /// level with the same name in the project settings. Falls back to the
        /// dropdown index clamped to the available levels.
        /// </summary>
        public static int ResolveQualityLevel(IReadOnlyList<string> qualityLevelNames, string optionLabel, int optionIndex)
        {
            if (qualityLevelNames == null || qualityLevelNames.Count == 0) return 0;
            for (int i = 0; i < qualityLevelNames.Count; i++)
                if (string.Equals(qualityLevelNames[i], optionLabel, StringComparison.OrdinalIgnoreCase))
                    return i;
            return Math.Max(0, Math.Min(optionIndex, qualityLevelNames.Count - 1));
        }

        /// <summary>
        /// The brightness option darkens the screen with a black overlay.
        /// A higher brightness means a more transparent overlay.
        /// </summary>
        public static float OverlayAlphaForBrightness(float brightness, float minBrightness, float maxBrightness)
        {
            if (maxBrightness <= minBrightness) return 0f;
            float clamped = Math.Min(maxBrightness, Math.Max(minBrightness, brightness));
            return maxBrightness - clamped;
        }

        /// <summary>
        /// The FOV slider (60-120, default 90) scales the field of view authored on the
        /// gameplay cameras instead of overwriting it, so the isometric framing of each
        /// level is kept at the default value.
        /// </summary>
        public static float ScaleFieldOfView(float authoredFieldOfView, float sliderValue, float referenceSliderValue = 90f)
        {
            if (referenceSliderValue <= 0f || sliderValue <= 0f) return authoredFieldOfView;
            float result = authoredFieldOfView * (sliderValue / referenceSliderValue);
            return Math.Min(179f, Math.Max(1f, result));
        }

        /// <summary>Clamps a frame-rate limit; 0 or less means "no limit" (-1 for Application.targetFrameRate).</summary>
        public static int ClampFrameRate(int requested, int min = 30, int max = 240)
        {
            if (requested <= 0) return -1;
            return Math.Min(max, Math.Max(min, requested));
        }
    }
}
