using System;
using AbilitySystem.ModifierMagnitude;
using AttributeSystem.Authoring;
using AttributeSystem.Components;

namespace AbilitySystem
{
    [Serializable]
    public struct GameplayEffectModifier
    {
        public AttributeScriptableObject Attribute;
        public EAttributeModifier ModifierOperator;
        public ModifierMagnitudeScriptableObject ModifierMagnitude;
        public float Multiplier;
    }



    public enum EAttributeModifier
    {
        Add, Multiply, Override
    }



}
