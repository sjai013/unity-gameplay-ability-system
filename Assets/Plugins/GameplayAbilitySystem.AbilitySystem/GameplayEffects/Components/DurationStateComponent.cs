using System.Runtime.CompilerServices;
using Unity.Entities;

namespace GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components
{
    /// <summary>
    /// Captures current state of a duration component
    /// </summary>
    public struct DurationStateComponent : IComponentData
    {
        public EDurationState State;

        /// <summary>
        /// Used to indicate that the tick has been addressed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetTickState()
        {
            State &= ~(EDurationState.TICKED_THIS_FRAME | EDurationState.TICKED_SINCE_RESET);
        }

        /// <summary>
        /// This Gameplay Effect ticked this frame
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TickedThisFrame()
        {
            return (State & EDurationState.TICKED_THIS_FRAME) != EDurationState.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkTick()
        {
            State |= (EDurationState.TICKED_THIS_FRAME | EDurationState.TICKED_SINCE_RESET);
        }

        public void MarkExpired()
        {
            State |= (EDurationState.EXPIRED);
        }
    }
}