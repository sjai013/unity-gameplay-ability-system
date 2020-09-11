using System.Runtime.CompilerServices;
using Unity.Entities;

namespace GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components
{
    /// <summary>
    /// Represents real-time ticks
    /// </summary>
    public struct TimeDurationComponent : IComponentData
    {
        public float Period;
        public float Duration;
        public float ElapsedPeriod;
        public float ElapsedDuration;
        public int TimesTicked;

        /// <summary>
        /// Ticks the duration component, decrementing the ElapsedDuration and ElapsedPeriod by the input duration.
        /// </summary>
        /// <param name="deltaTime">Time elapsed</param>
        /// <returns>True if <see cref="ElapsedPeriod"> has expired this execution.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Tick(float deltaTime)
        {
            ElapsedPeriod += deltaTime;
            ElapsedDuration += deltaTime;
            if (ElapsedPeriod >= Period)
            {
                // Decrement the period to reset it. 
                // Keep the time offset, so we can compensate for
                // time drifts
                ElapsedPeriod -= Period;
                TimesTicked++;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpired()
        {
            return ElapsedDuration >= Duration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeDurationComponent New(float period, float duration)
        {
            return new TimeDurationComponent()
            {
                Period = period,
                Duration = duration
            };
        }
    }
}