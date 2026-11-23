using System;

namespace IversTales.Core
{
    /// <summary>
    /// Health shown as hearts. With 40 health and 10 health per heart the player has
    /// four hearts; any amount of damage removes the hearts that are no longer covered
    /// (the original code compared health with exact values such as == 30, so damage
    /// that was not a multiple of 10 never removed a heart and never killed the player).
    /// </summary>
    public sealed class PlayerHealth
    {
        public int MaxHealth { get; }
        public int HealthPerHeart { get; }
        public int Current { get; private set; }
        public bool IsDead => Current <= 0;
        public int MaxHearts => (MaxHealth + HealthPerHeart - 1) / HealthPerHeart;

        /// <summary>Hearts that should be drawn as full.</summary>
        public int VisibleHearts => Current <= 0 ? 0 : (Current + HealthPerHeart - 1) / HealthPerHeart;

        /// <summary>Raised once, the first time health reaches zero.</summary>
        public event Action Died;

        public PlayerHealth(int maxHealth, int healthPerHeart)
        {
            if (maxHealth <= 0) throw new ArgumentOutOfRangeException(nameof(maxHealth));
            if (healthPerHeart <= 0) throw new ArgumentOutOfRangeException(nameof(healthPerHeart));
            MaxHealth = maxHealth;
            HealthPerHeart = healthPerHeart;
            Current = maxHealth;
        }

        /// <summary>Applies damage and returns the damage actually taken. Dead players take no damage.</summary>
        public int TakeDamage(int amount)
        {
            if (amount <= 0 || IsDead) return 0;
            int taken = Math.Min(amount, Current);
            Current -= taken;
            if (IsDead) Died?.Invoke();
            return taken;
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead) return;
            Current = Math.Min(MaxHealth, Current + amount);
        }

        public void Reset() => Current = MaxHealth;
    }
}
