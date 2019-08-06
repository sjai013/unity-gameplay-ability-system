using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.GameplayCues;
using GameplayAbilitySystem.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {

    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class GameplayEffect : ScriptableObject {

        [SerializeField]
        private GameplayEffectPolicy _gameplayEffectPolicy = new GameplayEffectPolicy();

        [SerializeField]
        private GameplayEffectTags _gameplayEffectTags = new GameplayEffectTags();

        public EffectPeriodicity Period;

        [SerializeField]
        public List<GameplayCue> GameplayCues = new List<GameplayCue>();

        public StackingPolicy StackingPolicy = new StackingPolicy();
        public GameplayEffectTags GameplayEffectTags { get => _gameplayEffectTags; }
        public GameplayEffectPolicy GameplayEffectPolicy { get => _gameplayEffectPolicy; }

        public IEnumerable<(GameplayTag Tag, GameplayEffect Effect)> GrantedEffectTags => GrantedTags.Select(x => (x, this));

        public bool ApplicationTagRequirementMet(IGameplayAbilitySystem AbilitySystem) {
            var requiredTagsPresent = true;
            var ignoredTagsAbsent = true;

            if (GameplayEffectTags.ApplicationTagRequirements.RequirePresence.Count > 0) {
                requiredTagsPresent = AbilitySystem.ActiveTags.Any(x => GameplayEffectTags.ApplicationTagRequirements.RequirePresence.Contains(x));
            }

            if (GameplayEffectTags.ApplicationTagRequirements.RequireAbsence.Count > 0) {
                ignoredTagsAbsent = !AbilitySystem.ActiveTags.Any(x => GameplayEffectTags.ApplicationTagRequirements.RequireAbsence.Contains(x));
            }

            return requiredTagsPresent && ignoredTagsAbsent;
        }

        public List<GameplayTag> GetOwningTags() {
            var tags = new List<GameplayTag>(_gameplayEffectTags.GrantedTags.Added.Count
                                            + _gameplayEffectTags.AssetTags.Added.Count);

            tags.AddRange(_gameplayEffectTags.GrantedTags.Added);
            tags.AddRange(_gameplayEffectTags.AssetTags.Added);

            return tags;
        }

        public List<GameplayTag> GrantedTags => _gameplayEffectTags.GrantedTags.Added;

        public bool ApplicationRequirementsPass(AbilitySystemComponent AbilitySystem) {
            // return _gameplayEffectTags.ApplicationTagRequirements.RequirePresence;

            return true;
        }

        public Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> CalculateModifierEffect(Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> Existing = null) {
            Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> modifierTotals;
            if (Existing == null) {
                modifierTotals = new Dictionary<AttributeType, Dictionary<EModifierOperationType, float>>();
            } else {
                modifierTotals = Existing;
            }

            foreach (var modifier in GameplayEffectPolicy.Modifiers) {
                if (!modifierTotals.TryGetValue(modifier.Attribute, out var modifierType)) {
                    // This attribute hasn't been recorded before, so create a blank new record
                    modifierType = new Dictionary<EModifierOperationType, float>();
                    modifierTotals.Add(modifier.Attribute, modifierType);
                }

                if (!modifierType.TryGetValue(modifier.ModifierOperation, out var value)) {
                    value = 0;
                    switch (modifier.ModifierOperation) {
                        case EModifierOperationType.Multiply:
                            value = 1;
                            break;

                        case EModifierOperationType.Divide:
                            value = 1;
                            break;

                        default:
                            value = 0;
                            break;
                    }
                    modifierType.Add(modifier.ModifierOperation, value);
                }

                switch (modifier.ModifierOperation) {
                    case EModifierOperationType.Add:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] += modifier.ScaledMagnitude;
                        break;

                    case EModifierOperationType.Multiply:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;

                    case EModifierOperationType.Divide:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;
                }
            }

            return modifierTotals;
        }

        public Dictionary<AttributeType, AttributeModificationValues> CalculateAttributeModification(IGameplayAbilitySystem AbilitySystem, Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> Modifiers, bool operateOnCurrentValue = false) {
            var attributeModification = new Dictionary<AttributeType, AttributeModificationValues>();

            foreach (var attribute in Modifiers) {
                if (!attribute.Value.TryGetValue(EModifierOperationType.Add, out var addition)) {
                    addition = 0;
                }

                if (!attribute.Value.TryGetValue(EModifierOperationType.Multiply, out var multiplication)) {
                    multiplication = 1;
                }

                if (!attribute.Value.TryGetValue(EModifierOperationType.Divide, out var division)) {
                    division = 1;
                }

                var oldAttributeValue = 0f;
                if (!operateOnCurrentValue) {
                    oldAttributeValue = AbilitySystem.GetNumericAttributeBase(attribute.Key);
                } else {
                    oldAttributeValue = AbilitySystem.GetNumericAttributeCurrent(attribute.Key);
                }

                var newAttributeValue = (oldAttributeValue + addition) * (multiplication / division);

                if (!attributeModification.TryGetValue(attribute.Key, out var values)) {
                    values = new AttributeModificationValues();
                    attributeModification.Add(attribute.Key, values);
                }

                values.NewAttribueValue += newAttributeValue;
                values.OldAttributeValue += oldAttributeValue;
            }

            return attributeModification;
        }

        public void ApplyInstantEffect_Self(IGameplayAbilitySystem Target) {
            ApplyInstantEffect_Target(Target, Target);
        }

        public void ApplyInstantEffect_Target(IGameplayAbilitySystem Instigator, IGameplayAbilitySystem Target) {
            var commandBuffer = World.Active.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();


            var entityManager = World.Active.EntityManager;
            var attributeMods = CalculateModifiers();

            foreach (var item in attributeMods) {
                var attributeModEntity = commandBuffer.CreateEntity();
                commandBuffer.AddComponent(attributeModEntity, new AttributeModifyComponent());

                // Get base attribute value
                var attrValue = Target.GetNumericAttributeBase(item.Value.attribute);
                var attributeModData = new AttributeModificationComponent() {
                    Add = item.Value.add,
                    Multiply = item.Value.multiply,
                    Divide = item.Value.divide,
                    Change = 0,
                    Source = Instigator.entity,
                    Target = Target.entity
                };
                //entityManager.AddComponent(attributeModEntity, typeof(HealthAttributeModifier));

                // Set appropriate attribute modifier
                switch (item.Key) {
                    case 0:
                        commandBuffer.AddComponent(attributeModEntity, new HealthAttributeModifier());
                        break;
                    case 1:
                        commandBuffer.AddComponent(attributeModEntity, new MaxHealthAttributeModifier());
                        break;
                    case 2:
                        commandBuffer.AddComponent(attributeModEntity, new ManaAttributeModifier());
                        break;
                    case 3:
                        commandBuffer.AddComponent(attributeModEntity, new MaxManaAttributeModifier());
                        break;
                }

                if (GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Instant) {
                    commandBuffer.AddComponent(attributeModEntity, new PermanentAttributeModification());
                } else {
                    commandBuffer.AddComponent(attributeModEntity, new TemporaryAttributeModification());
                    var gameplayEffectData = new GameplayEffectDurationComponent() {
                        WorldStartTime = Time.time,
                        Duration = GameplayEffectPolicy.DurationMagnitude,
                    };
                    commandBuffer.AddComponent(attributeModEntity, gameplayEffectData);
                }

                commandBuffer.AddComponent(attributeModEntity, attributeModData);
            }
        }

        public Dictionary<int, (AttributeType attribute, float add, float multiply, float divide)> CalculateModifiers() {

            Dictionary<int, (AttributeType attribute, float add, float multiply, float divide)> attributeMods = new Dictionary<int, (AttributeType attribute, float add, float multiply, float divide)>();

            foreach (var modifier in GameplayEffectPolicy.Modifiers) {
                var add = 0f;
                var multiply = 0f;
                var divide = 0f;
                if (modifier.ModifierOperation == Enums.EModifierOperationType.Add) add += modifier.ScaledMagnitude;
                if (modifier.ModifierOperation == Enums.EModifierOperationType.Multiply) multiply += modifier.ScaledMagnitude;
                if (modifier.ModifierOperation == Enums.EModifierOperationType.Divide) divide += modifier.ScaledMagnitude;

                if (!attributeMods.TryGetValue(modifier.Attribute.AttributeId, out var attrs)) {
                    attrs = (modifier.Attribute, 0, 0, 0);
                    attributeMods.Add(modifier.Attribute.AttributeId, attrs);
                }

                attrs.add += add;
                attrs.multiply += multiply;
                attrs.divide += divide;
                attrs.attribute = modifier.Attribute;
                attributeMods[modifier.Attribute.AttributeId] = attrs;
                //attributeMods.Add(modifier.Attribute.AttributeId, attrs);
            }

            return attributeMods;
        }
    }
}