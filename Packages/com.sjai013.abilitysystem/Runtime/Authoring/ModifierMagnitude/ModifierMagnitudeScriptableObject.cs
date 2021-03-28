using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.ModifierMagnitude
{
    public abstract class ModifierMagnitudeScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Function called when the spec is first initialised (e.g. by the Instigator/Source Ability System)
        /// </summary>
        /// <param name="spec">Gameplay Effect Spec</param>
        public abstract void Initialise(GameplayEffectSpec spec);

        /// <summary>
        /// Function called when the magnitude is calculated, usually after the target has been assigned
        /// </summary>
        /// <param name="spec">Gameplay Effect Spec</param>
        /// <returns></returns>
        public abstract float? CalculateMagnitude(GameplayEffectSpec spec);
    }
}
