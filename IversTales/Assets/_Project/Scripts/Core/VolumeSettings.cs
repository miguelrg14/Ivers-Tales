using System;
using System.Collections.Generic;

namespace IversTales.Core
{
    public enum VolumeChannel
    {
        Master,
        Music,
        Effects,
        Interface,
        Dialogue
    }

    /// <summary>
    /// Persists the five volume sliders of the options menu. Values are always
    /// stored as linear 0..1 numbers (the original version mixed decibels and
    /// linear values under the same key, which reset the volume to 0 on the
    /// second launch).
    /// </summary>
    public sealed class VolumeSettings
    {
        public const float DefaultVolume = 1f;

        static readonly Dictionary<VolumeChannel, string> Keys = new Dictionary<VolumeChannel, string>
        {
            { VolumeChannel.Master, "masterVolume" },
            { VolumeChannel.Music, "musicVolume" },
            { VolumeChannel.Effects, "sfxVolume" },
            { VolumeChannel.Interface, "uiVolume" },
            { VolumeChannel.Dialogue, "voiceVolume" },
        };

        readonly ISettingsStore store;

        public VolumeSettings(ISettingsStore store)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public static IEnumerable<VolumeChannel> Channels => Keys.Keys;

        public static string KeyFor(VolumeChannel channel) => Keys[channel];

        /// <summary>Saved linear volume (0..1), or the default if nothing was saved.</summary>
        public float Get(VolumeChannel channel)
        {
            float value = store.GetFloat(Keys[channel], DefaultVolume);
            return Clamp01(value);
        }

        public void Set(VolumeChannel channel, float linearVolume)
        {
            store.SetFloat(Keys[channel], Clamp01(linearVolume));
        }

        public void ResetAll()
        {
            foreach (var channel in Channels)
                Set(channel, DefaultVolume);
        }

        static float Clamp01(float value)
        {
            if (float.IsNaN(value)) return DefaultVolume;
            return Math.Min(1f, Math.Max(0f, value));
        }
    }
}
