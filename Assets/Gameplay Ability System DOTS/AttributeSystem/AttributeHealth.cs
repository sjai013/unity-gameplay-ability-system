using GameplayAbilitySystem.AttributeSystem.DOTS.Components;
using Unity.Entities;

namespace MyGameplayAbilitySystem.AttributeSystem.DOTS.Components
{
    public class AttributeHealth
    {
        public struct Current : IComponentData, IAttributeCurrentValue
        {
            public float _value;

            public float Value { get => _value; set => _value = value; }
        }

        public struct Base : IComponentData, IAttributeBaseValue
        {
            public float _value;
            public float Value { get => _value; set => _value = value; }
        }

        public struct Modifiers : IComponentData, IAttributeModifiers
        {
            public float _add;
            public float _multiply;
            public float _overwrite;

            public float Add { get => _add; set => _add = value; }
            public float Multiply { get => _multiply; set => _multiply = value; }
            public float Overwrite { get => _overwrite; set => _overwrite = value; }
        }
    }

}

