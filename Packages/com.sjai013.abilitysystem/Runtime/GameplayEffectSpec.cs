using System;
using AbilitySystem.Authoring;
using AttributeSystem.Components;

namespace AbilitySystem
{
    [Serializable]
    public class GameplayEffectSpec
    {
        /// <summary>
        /// Original gameplay effect that is the base for this spec
        /// </summary>
        public GameplayEffectScriptableObject GameplayEffect;

        /// <summary>
        /// 
        /// </summary>
        public float Duration;
        public GameplayEffectPeriod Period;
        public float Level;
        public AbilitySystemCharacter Source;
        public AbilitySystemCharacter Target;

        public GameplayEffectSpec()
        {
            for (var i = 0; i < GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                GameplayEffect.gameplayEffect.Modifiers[i].ModifierMagnitude.Initialise(this);
            }
        }

    }

}
