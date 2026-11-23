using System;
using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class VolumeSettingsTests
    {
        [Fact]
        public void Get_ReturnsDefault_WhenNothingWasSaved()
        {
            var settings = new VolumeSettings(new InMemorySettingsStore());

            foreach (var channel in VolumeSettings.Channels)
                Assert.Equal(VolumeSettings.DefaultVolume, settings.Get(channel));
        }

        [Fact]
        public void FirstLaunch_DoesNotPersistDecibels()
        {
            // Regression: the first launch saved Log10(1) * 20 = 0 under "masterVolume",
            // and the next launch read that 0 back as the slider value (muted game).
            var store = new InMemorySettingsStore();
            var firstLaunch = new VolumeSettings(store);
            firstLaunch.Set(VolumeChannel.Master, firstLaunch.Get(VolumeChannel.Master));

            var secondLaunch = new VolumeSettings(store);
            Assert.Equal(1f, secondLaunch.Get(VolumeChannel.Master));
        }

        [Fact]
        public void Set_PersistsAcrossInstances()
        {
            var store = new InMemorySettingsStore();
            new VolumeSettings(store).Set(VolumeChannel.Music, 0.3f);

            Assert.Equal(0.3f, new VolumeSettings(store).Get(VolumeChannel.Music));
        }

        [Theory]
        [InlineData(-0.5f, 0f)]
        [InlineData(1.5f, 1f)]
        public void Set_ClampsToZeroOne(float input, float expected)
        {
            var settings = new VolumeSettings(new InMemorySettingsStore());
            settings.Set(VolumeChannel.Effects, input);
            Assert.Equal(expected, settings.Get(VolumeChannel.Effects));
        }

        [Fact]
        public void ResetAll_RestoresDefaults()
        {
            var settings = new VolumeSettings(new InMemorySettingsStore());
            settings.Set(VolumeChannel.Dialogue, 0.1f);
            settings.Set(VolumeChannel.Interface, 0.2f);

            settings.ResetAll();

            Assert.Equal(1f, settings.Get(VolumeChannel.Dialogue));
            Assert.Equal(1f, settings.Get(VolumeChannel.Interface));
        }

        [Fact]
        public void Keys_KeepTheOriginalPlayerPrefsNames()
        {
            Assert.Equal("masterVolume", VolumeSettings.KeyFor(VolumeChannel.Master));
            Assert.Equal("musicVolume", VolumeSettings.KeyFor(VolumeChannel.Music));
            Assert.Equal("sfxVolume", VolumeSettings.KeyFor(VolumeChannel.Effects));
            Assert.Equal("uiVolume", VolumeSettings.KeyFor(VolumeChannel.Interface));
            Assert.Equal("voiceVolume", VolumeSettings.KeyFor(VolumeChannel.Dialogue));
        }

        [Fact]
        public void Constructor_RejectsNullStore()
        {
            Assert.Throws<ArgumentNullException>(() => new VolumeSettings(null));
        }
    }
}
