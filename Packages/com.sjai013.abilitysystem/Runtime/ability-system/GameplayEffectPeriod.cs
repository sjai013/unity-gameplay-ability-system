using System;

namespace AbilitySystem
{
    /// <summary>
    /// Period configuration for duration/infinite GE
    /// </summary>

    [Serializable]
    public struct GameplayEffectPeriod
    {
        /// <summary>
        /// Period at which to tick this GE
        /// </summary>
        public float Period;

        /// <summary>
        /// Whether to execute GE on first application (true) or wait until the first tick (false)
        /// </summary>
        public bool ExecuteOnApplication;
    }

}
