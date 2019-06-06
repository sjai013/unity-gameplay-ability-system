using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes
{

    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Attribute Set")]
    [System.Serializable]
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class AttributeSet : MonoBehaviour, IAttributeSet
    {
        [SerializeField]
        private AttributeChangeDataEvent _attributeValueChanged = new AttributeChangeDataEvent();
        /// <inheritdoc />
        public AttributeChangeDataEvent AttributeValueChanged => _attributeValueChanged;

        /// <inheritdoc />
        [SerializeField]
        private List<Attribute> _attributes;
        public List<Attribute> Attributes { get => _attributes; set => _attributes = value; }

        /// <inheritdoc />
        public AbilitySystemComponent GetOwningAbilitySystem()
        {
            return this.GetComponent<AbilitySystemComponent>();
        }

        /// <inheritdoc />
        public bool PreGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData)
        {
            return true;
        }

        /// <inheritdoc />
        public void PreAttributeBaseChange(IAttribute Attribute, ref float newMagnitude)
        {
            return;
        }

        /// <inheritdoc />
        public void PreAttributeChange(IAttribute Attribute, ref float NewValue)
        {
            return;
        }

        /// <inheritdoc />
        public void PostGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData)
        {
            return;
        }
    }
}
