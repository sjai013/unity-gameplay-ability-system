using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes {

    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Attribute Set")]
    [System.Serializable]
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class AttributeSet : MonoBehaviour, IAttributeSet, IConvertGameObjectToEntity {

        [SerializeField]
        private AttributeChangeDataEvent _attributeBaseValueChanged = default;

        /// <inheritdoc />
        public AttributeChangeDataEvent AttributeBaseValueChanged => _attributeBaseValueChanged;

        /// <inheritDoc />
        [SerializeField]
        private AttributeChangeDataEvent _attributeCurrentValueChanged = default;

        public AttributeChangeDataEvent AttributeCurrentValueChanged => _attributeCurrentValueChanged;

        /// <inheritdoc />
        [SerializeField]
        private List<Attribute> _attributes;

        public List<Attribute> Attributes { get => _attributes; set => _attributes = value; }

        [SerializeField]
        private BaseAttributeChangeHandler _preAttributeBaseChangeHandler = default;

        public BaseAttributeChangeHandler PreAttributeBaseChangeHandler => _preAttributeBaseChangeHandler;

        [SerializeField]
        private BaseAttributeChangeHandler _preAttributeChangeHandler = default;

        public BaseAttributeChangeHandler PreAttributeChangeHandler => _preAttributeChangeHandler;

        /// <inheritdoc />
        public AbilitySystemComponent GetOwningAbilitySystem() {
            return GetComponent<AbilitySystemComponent>();
        }

        /// <inheritdoc />
        public bool PreGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData) {
            return true;
        }

        /// <inheritdoc />
        public void PreAttributeBaseChange(IAttribute Attribute, ref float newMagnitude) {
            if (_preAttributeBaseChangeHandler != null) {
                _preAttributeBaseChangeHandler.OnAttributeChange(this, Attribute, ref newMagnitude);
            }
            return;
        }

        /// <inheritdoc />
        public void PreAttributeChange(IAttribute Attribute, ref float NewValue) {
            if (_preAttributeChangeHandler != null) {
                _preAttributeChangeHandler.OnAttributeChange(this, Attribute, ref NewValue);
            }
            return;
        }

        /// <inheritdoc />
        public void PostGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData) {
            return;
        }

        private void SetAttributeEntityProperty(Attribute attribute, ref AttributesComponent attributesComponent) {
            var baseAttributeComponent = new BaseAttributeComponent() {
                BaseValue = attribute.BaseValue,
                CurrentValue = attribute.CurrentValue
            };

            // Find the property in attribute component which matches the current attribute AttributeType (use name)
            var fieldName = attribute.AttributeType.name;
            var fieldInfo = typeof(AttributesComponent).GetField(fieldName);

            if (fieldInfo != null) {
                var fieldObjectConversion = fieldInfo.GetValue(attributesComponent);

                if (fieldObjectConversion is BaseAttributeComponent) {
                    object boxed = attributesComponent;
                    fieldInfo.SetValue(boxed, baseAttributeComponent);
                    attributesComponent = (AttributesComponent)boxed;
                }
            }
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var data = new AttributesComponent {
                Health = new BaseAttributeComponent {
                    //BaseValue = Attributes.FirstOrDefault(x => x.AttributeType.name == "Health").BaseValue,
                    //CurrentValue = Attributes.FirstOrDefault(x => x.AttributeType.name == "Health").CurrentValue
                    BaseValue = 0,
                    CurrentValue = 0,
                },
                Mana = new BaseAttributeComponent {
                    BaseValue = 0,
                    CurrentValue = 0,
                },
                MaxHealth = new BaseAttributeComponent {
                    BaseValue = 0,
                    CurrentValue = 0,
                },
                MaxMana = new BaseAttributeComponent {
                    BaseValue = 0,
                    CurrentValue = 0,
                },
            };

            for (var i = 0; i < Attributes.Count; i++) {
                SetAttributeEntityProperty(Attributes[i], ref data);
            }

            dstManager.AddComponentData(entity, data);
        }
    }
}