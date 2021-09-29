using System;
using System.Collections.Generic;
using AbilitySystem.Authoring;
using AttributeSystem.Components;

namespace AbilitySystem
{
    [Serializable]
    public class GameplayEffectSpec
    {
        public delegate void GameplayEffectEventHandler(GameplayEffectSpec sender);
        public event GameplayEffectEventHandler OnRemove;
        public event GameplayEffectEventHandler OnTick;
        public event GameplayEffectEventHandler OnApply;

        /// <summary>
        /// Original gameplay effect that is the base for this spec
        /// </summary>
        public GameplayEffect GameplayEffect { get; private set; }

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

        private GameplayCueScriptableObject.AbstractGameplayCueSpec[] m_GameplayCueSpec;

        public static GameplayEffectSpec CreateNew(GameplayEffect GameplayEffect, AbilitySystemCharacter Source, float Level = 1)
        {
            return new GameplayEffectSpec(GameplayEffect, Source, Level);
        }

        private GameplayEffectSpec(GameplayEffect GameplayEffect, AbilitySystemCharacter Source, float Level = 1)
        {
            this.GameplayEffect = GameplayEffect;
            this.Source = Source;
            this.Target = Source; // Target defaults to source, unless changed
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

            this.TimeUntilPeriodTick = this.GameplayEffect.GetPeriod().Period;
            // By setting the time to 0, we make sure it gets executed at first opportunity
            if (this.GameplayEffect.GetPeriod().ExecuteOnApplication)
            {
                this.TimeUntilPeriodTick = 0;
            }
            var gameplayCues = GameplayEffect.GetGameplayCues();
            m_GameplayCueSpec = new GameplayCueScriptableObject.AbstractGameplayCueSpec[gameplayCues.Length];

            for (var i = 0; i < m_GameplayCueSpec.Length; i++)
            {
                if (gameplayCues[i] == null) continue;
                m_GameplayCueSpec[i] = gameplayCues[i].CreateSpec(this);
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
                this.TimeUntilPeriodTick = this.GameplayEffect.GetPeriod().Period;

                // Check to make sure period is valid, otherwise we'd just end up executing every frame
                if (this.GameplayEffect.GetPeriod().Period > 0)
                {
                    executePeriodicTick = true;
                    RaiseOnTickEvent();
                }
            }

            return this;
        }

        public GameplayEffectSpec SetLevel(float level)
        {
            this.Level = level;
            return this;
        }

        public void RaiseOnApplyEvent()
        {
            OnApply?.Invoke(this);
        }

        private void RaiseOnTickEvent()
        {
            OnTick?.Invoke(this);
        }

        public void RaiseOnRemoveEvent()
        {
            OnRemove?.Invoke(this);
        }

    }

}
