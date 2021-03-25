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
        public GameplayEffectScriptableObject GameplayEffect { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float Duration { get; private set; }
        public GameplayEffectPeriod Period { get; private set; }
        public float Level { get; private set; }
        public AbilitySystemCharacter Source { get; private set; }
        public AbilitySystemCharacter Target { get; private set; }
        public AttributeValue? SourceCapturedAttribute = null;

        public GameplayEffectSpec(GameplayEffectScriptableObject GameplayEffect, AbilitySystemCharacter Source, float Duration = 0, float Level = 1)
        {
            this.GameplayEffect = GameplayEffect;
            this.Source = Source;
            for (var i = 0; i < this.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                this.GameplayEffect.gameplayEffect.Modifiers[i].ModifierMagnitude.Initialise(this);
            }
        }

        public GameplayEffectSpec SetTarget(AbilitySystemCharacter target)
        {
            this.Target = target;
            return this;
        }

        public GameplayEffectSpec SetDuration(float duration)
        {
            this.Duration = duration;
            return this;
        }

        public GameplayEffectSpec SetLevel(float level)
        {
            this.Level = level;
            return this;
        }

    }

}
