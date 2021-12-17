using System;
using GameplayAbilitySystem.AttributeSystem.DOTS.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem.AttributeSystem.DOTS.Components
{

    public struct AttributeMaxHealth : IAttributeData
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

}

