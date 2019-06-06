using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.Events;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.GameplayEffects;
using UnityEngine;
using UnityEngine.Events;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Abilities.AbilityActivations;
using System.Linq;

namespace GameplayAbilitySystem.Abilities
{
    /// <inheritdoc />
    [AddComponentMenu("Ability System/Ability")]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability")]
    public class GameplayAbility : ScriptableObject, IGameplayAbility
    {

        [SerializeField]
        private GameplayAbilityTags _tags = new GameplayAbilityTags();

        [SerializeField]
        private GameplayCost _gameplayCost = new GameplayCost();

        [SerializeField]
        private List<GameplayCooldown> _gameplayCooldowns = new List<GameplayCooldown>();

        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityCommitted = new GenericAbilityEvent();


        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityCancelled = new GenericAbilityEvent();


        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityEnded = new GenericAbilityEvent();

        [SerializeField]
        private AbstractAbilityActivation _abilityLogic;



        /// <inheritdoc />
        public IAbilityTags Tags => _tags;
        /// <inheritdoc />
        public IGameplayCost GameplayCost => _gameplayCost;
        /// <inheritdoc />
        public List<GameplayCooldown> GameplayCooldowns => _gameplayCooldowns;
        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;
        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityCancelled => _onGameplayAbilityCancelled;
        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;
        /// <inheritdoc />
        public AbstractAbilityActivation AbilityLogic => _abilityLogic;



        void Start()
        {
        }



        protected void ApplyGameplayEffectToTarget(GameplayEffect effect, AbilitySystemComponent target)
        {
            //
        }

        /// <inheritdoc />
        public virtual void ActivateAbility(IGameplayAbilitySystem AbilitySystem)
        {
            _abilityLogic.ActivateAbility(AbilitySystem, this);
            ApplyCooldown(AbilitySystem);
        }

        /// <inheritdoc />
        public virtual bool IsAbilityActivatable(IGameplayAbilitySystem AbilitySystem)
        {
            return PlayerHasResourceToCast(AbilitySystem) && AbilityOffCooldown(AbilitySystem);
        }

        /// <summary>
        /// Applies the ability cost, decreasing the specified cost resource from the player.
        /// If player doesn't have the required resource, the resource goes to negative (or clamps to 0)
        /// </summary>
        protected void ApplyCost()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies cooldown.  Cooldown is applied even if the  ability is already
        /// on cooldown
        /// </summary>
        protected void ApplyCooldown(IGameplayAbilitySystem abilitySystem)
        {
            for (var i = 0; i < this.GameplayCooldowns.Count; i++)
            {
                abilitySystem.AddGameplayEffectToActiveList(this.GameplayCooldowns[i].CooldownGameplayEffect);
            }
        }

        /// <inheritdoc />
        public void EndAbility(IGameplayAbilitySystem AbilitySystem)
        {
            _onGameplayAbilityEnded.Invoke(this);

            // Ability finished.  Remove all listeners.
            _onGameplayAbilityEnded.RemoveAllListeners();

            // TODO: Remove tags added by this ability

            // TODO: Cancel all tasks?

            // TODO: Remove gameplay cues

            // TODO: Cancel ability

            // TODO: Remove blocking/cancelling Gameplay Tags

            // Tell ability system ability has ended
            AbilitySystem.NotifyAbilityEnded(this);
        }


        /// <inheritdoc />
        public bool PlayerHasResourceToCast(IGameplayAbilitySystem AbilitySystem)
        {
            return true;
        }

        /// <inheritdoc />
        public bool CommitAbility(IGameplayAbilitySystem AbilitySystem)
        {
            if (!IsAbilityActivatable(AbilitySystem)) return false;
            // this.abilitySystem.OnGameplayAbilityActivated.Invoke(this);
            ApplyCost();
            return true;
        }

        /// <inheritdoc />
        public bool AbilityOffCooldown(IGameplayAbilitySystem AbilitySystem)
        {
            var cooldownTags = this.GetAbilityCooldownTags();

            // Check if the ability system has any of these tags.  Since tags are provided
            // by game effects, that means we need to iterate through all active game effects
            // and check if any of them provide these tags

            // Check each active gameplay effect.  If we have a match, store the reference
            foreach (var gameplayEffect in AbilitySystem.ActiveGameplayEffects)
            {
                if (gameplayEffect.Effect.GetOwningTags().Intersect(cooldownTags).Count() > 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public List<GameplayTag> GetAbilityCooldownTags()
        {
            var tags = new List<GameplayTag>();
            for (var i = 0; i < this._gameplayCooldowns.Count; i++)
            {
                tags.AddRange(this._gameplayCooldowns[i].GetCooldownTags());
            }

            return tags;
        }

    }
}
