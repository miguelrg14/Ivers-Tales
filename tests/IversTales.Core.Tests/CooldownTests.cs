using System;
using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class CooldownTests
    {
        [Fact]
        public void StartsReady_AndBlocksUntilElapsed()
        {
            var cooldown = new Cooldown(2f);
            Assert.True(cooldown.TryTrigger());
            Assert.False(cooldown.TryTrigger());

            cooldown.Tick(1.5f);
            Assert.False(cooldown.IsReady);

            cooldown.Tick(0.5f);
            Assert.True(cooldown.IsReady);
            Assert.True(cooldown.TryTrigger());
        }

        [Fact]
        public void Tick_IgnoresNegativeDelta()
        {
            var cooldown = new Cooldown(1f);
            cooldown.TryTrigger();
            cooldown.Tick(-5f);
            Assert.Equal(1f, cooldown.Remaining);
        }

        [Fact]
        public void Constructor_RejectsNegativeDuration()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Cooldown(-1f));
        }
    }
}
