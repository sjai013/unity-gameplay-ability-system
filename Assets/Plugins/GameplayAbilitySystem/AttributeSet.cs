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
        private AttributeChangeDataEvent _attributeBaseValueChanged = new AttributeChangeDataEvent();
        /// <inheritdoc />
        public AttributeChangeDataEvent AttributeBaseValueChanged => _attributeBaseValueChanged;

        /// <inheritdoc />
        [SerializeField]
        private List<Attribute> _attributes;
        public List<Attribute> Attributes { get => _attributes; set => _attributes = value; }

        /// <inheritDoc />
        [SerializeField]
        private AttributeChangeDataEvent _attributeCurrentValueChanged = default;
        public AttributeChangeDataEvent AttributeCurrentValueChanged => _attributeCurrentValueChanged;

        [SerializeField]
        private BaseAttributeChangeHandler _preAttributeBaseChangeHandler = default;
        public BaseAttributeChangeHandler PreAttributeBaseChangeHandler => _preAttributeBaseChangeHandler;

        [SerializeField]
        private BaseAttributeChangeHandler _preAttributeChangeHandler = default;
        public BaseAttributeChangeHandler PreAttributeChangeHandler => _preAttributeChangeHandler;

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
            if (_preAttributeBaseChangeHandler != null)
            {
                _preAttributeBaseChangeHandler.OnAttributeChange(Attribute, ref newMagnitude);
            }
            return;
        }

        /// <inheritdoc />
        public void PreAttributeChange(IAttribute Attribute, ref float NewValue)
        {
            if (_preAttributeChangeHandler != null)
            {
                _preAttributeChangeHandler.OnAttributeChange(Attribute, ref NewValue);
            }
            return;
        }

        /// <inheritdoc />
        public void PostGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData)
        {
            return;
        }
    }
}
