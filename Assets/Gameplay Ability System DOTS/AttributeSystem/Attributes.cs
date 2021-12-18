using System;
using System.ComponentModel;
using GameplayAbilitySystem.AttributeSystem.DOTS.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem.AttributeSystem.DOTS.Components
{

    public struct AttributeStrength : IAttributeData, IComponentData
    {
        public PlayerAttribute Value;
    }

    public struct AttributeHealth : IAttributeData, IComponentData
    {
        public PlayerAttribute Value;
    }

    public struct AttributeMana : IAttributeData, IComponentData
    {
        public PlayerAttribute Value;
    }

    public struct AttributeMaxMana : IAttributeData, IComponentData
    {
        public PlayerAttribute Value;
    }

    public struct AttributeSpeed : IAttributeData, IComponentData
    {
        public PlayerAttribute Value;
    }

    public struct AttributeMaxHealth : IAttributeDerivedData, IComponentData
    {
        public PlayerAttribute Value;
        public AttributeStrength Strength;
        public float CalculateBase()
        {
            return Strength.Value.BaseValue * 1.5f;
        }
    }

}

