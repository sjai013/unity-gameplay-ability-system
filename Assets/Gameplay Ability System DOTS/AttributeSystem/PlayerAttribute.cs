using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;


namespace GameplayAbilitySystem.AttributeSystem.DOTS.Components
{
    public interface IAttributeData : IComponentData { }

    [Serializable]
    public struct PlayerAttribute
    {
        public float BaseValue;
        public float CurrentValue;
        public Modifiers Modifiers;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float CalculateCurrentValue()
        {
            return (BaseValue + Modifiers.Add) * (Modifiers.Multiply + 1);
        }

        public static PlayerAttribute Initialise(float baseValue)
        {
            return new PlayerAttribute()
            {
                BaseValue = baseValue
            };
        }

    }

    [Serializable]
    public struct Modifiers
    {
        public float Add;
        public float Multiply;
    }

}