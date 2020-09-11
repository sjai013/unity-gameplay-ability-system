using System.Runtime.CompilerServices;
using Unity.Entities;

namespace GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components {
    /// <summary>
    /// Represents ticking in whole intervals (like turns in a turn-based game)
    /// </summary>
    public struct TurnDurationComponent : IComponentData {
        public int Period;
        public int Duration;
        public int ElapsedPeriod;
        public int ElapsedDuration;
        public int TimesTicked;

        /// <summary>
        /// Ticks the duration component, decrementing the ElapsedDuration and ElapsedPeriod by the input duration.
        /// </summary>
        /// <param name="step">Time elapsed</param>
        /// <returns>True if <see cref="ElapsedPeriod"> has expired this execution.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Tick(int step = 1)
        {
            ElapsedPeriod += step;
            ElapsedDuration += step;
            if (ElapsedPeriod >= Period) {
                // Decrement the period to reset it. 
                // Keep the time offset, so we can compensate for
                // time drifts
                ElapsedPeriod -= Period;
                TimesTicked++;
                return true;
            }
            return false;
        }
    }
}