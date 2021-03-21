using System;
using AttributeSystem.Authoring;

namespace AttributeSystem.Components
{
    [Serializable]
    public struct AttributeValue
    {
        public AttributeScriptableObject Attribute;
        public float BaseValue;
        public float CurrentValue;
        public AttributeModifier Modifier;
    }

    [Serializable]
    public struct AttributeModifier
    {
        public float Add;
        public float Multiply;
        public float Override;
        public AttributeModifier Combine(AttributeModifier other)
        {
            other.Add += Add;
            other.Multiply += Multiply;
            other.Override = Override;
            return other;
        }
    }

}
