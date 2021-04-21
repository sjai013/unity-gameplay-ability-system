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
        public float DurationRemaining { get; private set; }

        public float TotalDuration { get; private set; }
        public GameplayEffectPeriod PeriodDefinition { get; private set; }
        public float TimeUntilPeriodTick { get; private set; }
        public float Level { get; private set; }
        public AbilitySystemCharacter Source { get; private set; }
        public AbilitySystemCharacter Target { get; private set; }
        public AttributeValue? SourceCapturedAttribute = null;

        public static GameplayEffectSpec CreateNew(GameplayEffectScriptableObject GameplayEffect, AbilitySystemCharacter Source, float Level = 1)
        {
            return new GameplayEffectSpec(GameplayEffect, Source, Level);
        }

        private GameplayEffectSpec(GameplayEffectScriptableObject GameplayEffect, AbilitySystemCharacter Source, float Level = 1)
        {
            this.GameplayEffect = GameplayEffect;
            this.Source = Source;
            for (var i = 0; i < this.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                this.GameplayEffect.gameplayEffect.Modifiers[i].ModifierMagnitude.Initialise(this);
            }
            this.Level = Level;
            if (this.GameplayEffect.gameplayEffect.DurationModifier)
            {
                this.DurationRemaining = this.GameplayEffect.gameplayEffect.DurationModifier.CalculateMagnitude(this).GetValueOrDefault() * this.GameplayEffect.gameplayEffect.DurationMultiplier;
                this.TotalDuration = this.DurationRemaining;
            }

            this.TimeUntilPeriodTick = this.GameplayEffect.Period.Period;
            // By setting the time to 0, we make sure it gets executed at first opportunity
            if (this.GameplayEffect.Period.ExecuteOnApplication)
            {
                this.TimeUntilPeriodTick = 0;
            }
        }

        public GameplayEffectSpec SetTarget(AbilitySystemCharacter target)
        {
            this.Target = target;
            return this;
        }

        public void SetTotalDuration(float totalDuration)
        {
            this.TotalDuration = totalDuration;
        }

        public GameplayEffectSpec SetDuration(float duration)
        {
            this.DurationRemaining = duration;
            return this;
        }

        public GameplayEffectSpec UpdateRemainingDuration(float deltaTime)
        {
            this.DurationRemaining -= deltaTime;
            return this;
        }

        public GameplayEffectSpec TickPeriodic(float deltaTime, out bool executePeriodicTick)
        {
            this.TimeUntilPeriodTick -= deltaTime;
            executePeriodicTick = false;
            if (this.TimeUntilPeriodTick <= 0)
            {
                this.TimeUntilPeriodTick = this.GameplayEffect.Period.Period;

                // Check to make sure period is valid, otherwise we'd just end up executing every frame
                if (this.GameplayEffect.Period.Period > 0)
                {
                    executePeriodicTick = true;
                }
            }

            return this;
        }

        public GameplayEffectSpec SetLevel(float level)
        {
            this.Level = level;
            return this;
        }

    }

}
