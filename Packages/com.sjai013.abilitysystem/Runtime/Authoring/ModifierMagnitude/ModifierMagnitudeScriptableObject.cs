using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.ModifierMagnitude
{
    public abstract class ModifierMagnitudeScriptableObject : ScriptableObject
    {
        public virtual void Initialise(GameplayEffectSpec spec) { }
        public abstract float? CalculateMagnitude(GameplayEffectSpec spec);
    }
}
