using System;
using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class PlayerHealthTests
    {
        [Fact]
        public void StartsWithFullHearts()
        {
            var health = new PlayerHealth(40, 10);
            Assert.Equal(4, health.MaxHearts);
            Assert.Equal(4, health.VisibleHearts);
            Assert.False(health.IsDead);
        }

        [Theory]
        [InlineData(10, 3)]
        [InlineData(5, 4)]
        [InlineData(15, 3)]
        [InlineData(25, 2)]
        [InlineData(39, 1)]
        public void Hearts_FollowAnyDamageAmount(int damage, int expectedHearts)
        {
            var health = new PlayerHealth(40, 10);
            health.TakeDamage(damage);
            Assert.Equal(expectedHearts, health.VisibleHearts);
        }

        [Fact]
        public void DamageNotMultipleOfTen_StillKillsThePlayer()
        {
            // Regression: health was compared with == 0, so 40 - 15 - 15 - 15 = -5 never died.
            var health = new PlayerHealth(40, 10);
            health.TakeDamage(15);
            health.TakeDamage(15);
            health.TakeDamage(15);

            Assert.True(health.IsDead);
            Assert.Equal(0, health.Current);
            Assert.Equal(0, health.VisibleHearts);
        }

        [Fact]
        public void Died_IsRaisedOnlyOnce()
        {
            var health = new PlayerHealth(40, 10);
            int deaths = 0;
            health.Died += () => deaths++;

            health.TakeDamage(50);
            health.TakeDamage(10);

            Assert.Equal(1, deaths);
        }

        [Fact]
        public void TakeDamage_ReturnsDamageActuallyTaken()
        {
            var health = new PlayerHealth(40, 10);
            Assert.Equal(30, health.TakeDamage(30));
            Assert.Equal(10, health.TakeDamage(30));
            Assert.Equal(0, health.TakeDamage(30));
            Assert.Equal(0, new PlayerHealth(40, 10).TakeDamage(-5));
        }

        [Fact]
        public void Heal_IsCappedAndDoesNotReviveTheDead()
        {
            var health = new PlayerHealth(40, 10);
            health.TakeDamage(20);
            health.Heal(100);
            Assert.Equal(40, health.Current);

            health.TakeDamage(40);
            health.Heal(10);
            Assert.True(health.IsDead);
        }

        [Fact]
        public void Reset_RestoresFullHealth()
        {
            var health = new PlayerHealth(40, 10);
            health.TakeDamage(40);
            health.Reset();
            Assert.Equal(40, health.Current);
        }

        [Fact]
        public void Constructor_ValidatesArguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlayerHealth(0, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlayerHealth(40, 0));
        }
    }
}
