using System;
using System.Collections.Generic;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTag.Authoring;
using UnityEngine;

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

        public bool IsActive { get; private set; }

        public ModifierContainer[] modifiers;

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

        public void RaiseOnTickEvent()
        {
            OnTick?.Invoke(this);
        }

        public void RaiseOnRemoveEvent()
        {
            OnRemove?.Invoke(this);
        }

        public void UpdateState()
        {
            this.IsActive = OngoingRequirementsPassed();
        }

        /// <summary>
        /// Checks if the target of this gameplay effect passes the ongoing tag requirements check
        /// </summary>
        /// <returns>True, if the ongoing tag requirements pass.  False otherwise.</returns>
        public bool OngoingRequirementsPassed()
        {
            var ongoingTags = this.GameplayEffect.GetGameplayEffectTags().OngoingTagRequirements;
            if (AscHasAllTags(this.Target, ongoingTags.RequireTags)
                && AscHasNoneTags(this.Target, ongoingTags.IgnoreTags)
            )
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Checks if an Ability System Character has all the listed tags
        /// </summary>
        /// <param name="asc">Ability System Character</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns>True, if the Ability System Character has all tags</returns>
        protected virtual bool AscHasAllTags(AbilitySystemCharacter asc, GameplayTagScriptableObject.GameplayTag[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                bool requirementPassed = false;
                Debug.Log(asc.AppliedTags.Count);
                for (var iAscTag = 0; iAscTag < asc.AppliedTags.Count; iAscTag++)
                {
                    if (asc.AppliedTags[iAscTag].TagData == abilityTag)
                    {
                        requirementPassed = true;
                        continue;
                    }
                }
                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if an Ability System Character has none of the listed tags
        /// </summary>
        /// <param name="asc">Ability System Character</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns>True, if the Ability System Character has none of the tags</returns>
        protected virtual bool AscHasNoneTags(AbilitySystemCharacter asc, GameplayTagScriptableObject.GameplayTag[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                bool requirementPassed = true;
                for (var iAscTag = 0; iAscTag < asc.AppliedTags.Count; iAscTag++)
                {
                    if (asc.AppliedTags[iAscTag].TagData == abilityTag)
                    {
                        requirementPassed = false;
                    }
                }
                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }
            return true;
        }


        public class ModifierContainer
        {
            public AttributeScriptableObject Attribute;
            public AttributeModifier Modifier;
        }


    }

}
