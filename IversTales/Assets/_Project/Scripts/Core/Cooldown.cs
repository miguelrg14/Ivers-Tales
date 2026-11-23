using System;

namespace IversTales.Core
{
    /// <summary>Simple cooldown driven by elapsed time, used by the player abilities.</summary>
    public sealed class Cooldown
    {
        public float Duration { get; set; }
        public float Remaining { get; private set; }
        public bool IsReady => Remaining <= 0f;

        public Cooldown(float duration)
        {
            if (duration < 0f) throw new ArgumentOutOfRangeException(nameof(duration));
            Duration = duration;
        }

        /// <summary>Starts the cooldown if it is ready. Returns false if it was still running.</summary>
        public bool TryTrigger()
        {
            if (!IsReady) return false;
            Remaining = Duration;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || Remaining <= 0f) return;
            Remaining = Math.Max(0f, Remaining - deltaTime);
        }
    }
}
