using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Events;
using GameplayAbilitySystem.GameplayCues;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;


namespace GameplayAbilitySystem {


    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Ability System")]
    public class AbilitySystemComponent : MonoBehaviour, IGameplayAbilitySystem, IConvertGameObjectToEntity {

        static public Dictionary<EAbility, Type> Abilities = new Dictionary<EAbility, Type>()
    {
        { EAbility.FireAbility, typeof(Abilities.Fire.FireAbilityComponent) },
        { EAbility.HealAbility, typeof(Abilities.Heal.HealAbilityComponent) },
    };
        public Transform TargettingLocation;

        [SerializeField]
        private readonly GenericAbilityEvent _onGameplayAbilityActivated = new GenericAbilityEvent();

        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityActivated => _onGameplayAbilityActivated;

        [SerializeField]
        private readonly GenericAbilityEvent _onGameplayAbilityEnded = new GenericAbilityEvent();

        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;

        [SerializeField]
        private readonly GameplayEvent _onGameplayEvent = new GameplayEvent();

        /// <inheritdoc />
        public GameplayEvent OnGameplayEvent => _onGameplayEvent;

        [SerializeField]
        protected ActiveGameplayEffectsContainer _activeGameplayEffectsContainer;

        /// <inheritdoc />
        public ActiveGameplayEffectsContainer ActiveGameplayEffectsContainer => _activeGameplayEffectsContainer;

        [SerializeField]
        protected List<IGameplayAbility> _runningAbilities = new List<IGameplayAbility>();

        /// <inheritdoc />
        public List<IGameplayAbility> RunningAbilities => _runningAbilities;

        [SerializeField]
        protected GenericGameplayEffectEvent _onEffectAdded = new GenericGameplayEffectEvent();

        /// <inheritdoc />
        public GenericGameplayEffectEvent OnEffectAdded => _onEffectAdded;

        [SerializeField]
        protected GenericGameplayEffectEvent _onEffectRemoved = new GenericGameplayEffectEvent();

        /// <inheritdoc />
        public GenericGameplayEffectEvent OnEffectRemoved => _onEffectRemoved;

        [SerializeField]
        private readonly GenericAbilityEvent _onGameplayAbilityCommitted = new GenericAbilityEvent();

        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;

        private Animator _animator;

        public Entity entity { private set; get; }

        public Animator Animator => _animator;

        public IEnumerable<GameplayTag> ActiveTags {
            get {
                return ActiveGameplayEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetActiveEffects()
                            .SelectMany(x => x.Effect.GameplayEffectTags.GrantedTags.Added)
                            .Union(AbilityGrantedTags);
            }
        }

        private IEnumerable<GameplayTag> AbilityGrantedTags => _runningAbilities.SelectMany(x => x.Tags.ActivationOwnedTags.Added);

        public IEnumerable<(GameplayTag Tag, ActiveGameplayEffectData GrantingEffect)> ActiveTagsByActiveGameplayEffect {
            get {
                var activeEffects = ActiveGameplayEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetActiveEffects();

                if (activeEffects == null) return new List<(GameplayTag, ActiveGameplayEffectData)>();

                var activeEffectsTags = activeEffects.SelectMany(x =>
                     x.Effect.GrantedTags
                     .Select(y => (y, x)));

                return activeEffectsTags;
            }
        }

        public void Awake() {
            _activeGameplayEffectsContainer = new ActiveGameplayEffectsContainer(this);
            _animator = GetComponent<Animator>();
        }

        /// <inheritdoc />
        public Transform GetActor() {
            return transform;
        }

        private void Update() {
        }

        /// <inheritdoc />
        public void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload) {
            /**
             * TODO: Handle triggered abilities
             * Search component for all abilities that are automatically triggered from a gameplay event
             */

            OnGameplayEvent.Invoke(EventTag, Payload);
        }

        /// <inheritdoc />
        public void NotifyAbilityEnded(GameplayAbility ability) {
            _runningAbilities.Remove(ability);
        }

        /// <inheritdoc />
        public bool TryActivateAbility(EAbility Ability, AbilitySystemComponent Source, AbilitySystemComponent Target) {
            if (World.Active.EntityManager.HasComponent<CastingAbilityTagComponent>(Source.entity)) return false;

            Type abilityType = Abilities[Ability];

            // World.Active.EntityManager.AddComponent(entity, typeof(CastingAbilityTagComponent));
            // var abilityEntity = World.Active.EntityManager.CreateEntity(typeof(CheckAbilityConstraintsComponent), 
            //                                                             typeof(FireAbility), 
            //                                                             typeof(FireAbilitySystem.AbilityCost));
            var tryActiveAbilityEntity = World.Active.EntityManager.CreateEntity(abilityType, typeof(AbilitySourceTargetComponent), typeof(AbilityStateComponent), typeof(AbilityComponent));
            AbilitySourceTargetComponent abilitySourceTarget = new AbilitySourceTargetComponent() { Source = Source.entity, Target = Target.entity };
            AbilityComponent abilityComponent = new AbilityComponent {
                Ability = Ability
            };
            AbilityStateComponent abilityState = new AbilityStateComponent
            {
                State = EAbilityState.TryActivate
            };

            World.Active.EntityManager.SetComponentData(tryActiveAbilityEntity, abilitySourceTarget);
            World.Active.EntityManager.SetComponentData(tryActiveAbilityEntity, abilityState);
            World.Active.EntityManager.SetComponentData(tryActiveAbilityEntity, abilityComponent);

            return true;
        }

        /// <inheritdoc />
        public bool CanActivateAbility(IGameplayAbility Ability) {
            // Check if this ability is already active on this ASC
            if (_runningAbilities.Contains(Ability)) {
                return false;
            }

            return true;
        }

        public async void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level)> BatchedGameplayEffects) {
            var instantEffects = BatchedGameplayEffects.Where(x => x.Effect.GameplayEffectPolicy.DurationPolicy == EDurationPolicy.Instant);
            var durationalEffects = BatchedGameplayEffects.Where(
                x =>
                    x.Effect.GameplayEffectPolicy.DurationPolicy == EDurationPolicy.HasDuration ||
                    x.Effect.GameplayEffectPolicy.DurationPolicy == EDurationPolicy.Infinite
                    );

            // Apply instant effects
            foreach (var item in instantEffects) {
                if (await ApplyGameEffectToTarget(item.Effect, item.Target)) {
                    // item.Target.AddGameplayEffectToActiveList(Effect);
                }
            }

            // Apply durational effects
            foreach (var effect in durationalEffects) {
                if (await ApplyGameEffectToTarget(effect.Effect, effect.Target)) {
                }
            }
        }

        /// <inheritdoc />
        public Task<GameplayEffect> ApplyGameEffectToTarget(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level = 0) {



            if (!Effect.ApplicationTagRequirementMet(Target)) {
                return null;
            }

            // If this is a non-instant gameplay effect (i.e. it will modify the current value, not the base value)

            // If this is an instant gameplay effect (i.e. it will modify the base value)

            // Handling Instant effects is different to handling HasDuration and Infinite effects
            if (Effect.GameplayEffectPolicy.DurationPolicy == EDurationPolicy.Instant) {
                Effect.ApplyInstantEffect_Self(Target);
            } else {
                // Durational effects require attention to many more things than instant effects
                // Such as stacking and effect durations
                var EffectData = new ActiveGameplayEffectData(Effect, this, Target);
                _ = Target.ActiveGameplayEffectsContainer.ApplyGameEffect(EffectData);
            }

            // Remove all effects which have tags defined as "Remove Gameplay Effects With Tag".
            // We do this by setting the expiry time on the effect to make it end prematurely
            // This is accomplished by finding all effects which grant these tags, and then adjusting start time
            var tagsToRemove = Effect.GameplayEffectTags.RemoveGameplayEffectsWithTag.Added;
            var activeGEs = Target.ActiveTagsByActiveGameplayEffect
                                    .Where(x => tagsToRemove.Any(y => x.Tag == y.Tag))
                                    .Join(tagsToRemove, x => x.Tag, x => x.Tag, (x, y) => new { Tag = x.Tag, EffectData = x.GrantingEffect, StacksToRemove = y.StacksToRemove })
                                    .OrderBy(x => x.EffectData.CooldownTimeRemaining);

            Dictionary<GameplayEffect, int> StacksRemoved = new Dictionary<GameplayEffect, int>();
            foreach (var GE in activeGEs) {
                if (!StacksRemoved.ContainsKey(GE.EffectData.Effect)) {
                    StacksRemoved.Add(GE.EffectData.Effect, 0);
                }
                var stacksRemoved = StacksRemoved[GE.EffectData.Effect];
                if (GE.StacksToRemove == 0 || stacksRemoved < GE.StacksToRemove) {
                    GE.EffectData.ForceEndEffect();
                }

                StacksRemoved[GE.EffectData.Effect]++;
            }

            var gameplayCues = Effect.GameplayCues;
            // Execute gameplay cue
            for (var i = 0; i < gameplayCues.Count; i++) {
                var cue = gameplayCues[i];
                cue.HandleGameplayCue(Target.GetActor().gameObject, new GameplayCueParameters(null, null, null), EGameplayCueEvent.OnActive);
            }

            return Task.FromResult(Effect);
        }

        /// <inheritdoc />
        public float GetNumericAttributeBase(AttributeType AttributeType) {
            var attributeSet = GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            if (attribute == null) return 0;
            return attribute.BaseValue;
        }

        /// <inheritdoc />
        public float GetNumericAttributeCurrent(AttributeType AttributeType) {
            var attributeSet = GetComponent<AttributeSet>();
            return attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType).CurrentValue;
        }

        public void SetNumericAttributeBase(AttributeType AttributeType, float modifier) {
            var attributeSet = GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            var newValue = modifier;
            attribute.SetAttributeBaseValue(attributeSet, ref newValue);

        }

        public void SetNumericAttributeCurrent(AttributeType AttributeType, float NewValue) {
            var attributeSet = GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            attribute.SetAttributeCurrentValue(attributeSet, ref NewValue);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            this.entity = entity;
        }
    }
}