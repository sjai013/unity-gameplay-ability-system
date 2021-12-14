
using System.Collections.Generic;
using GameplayAbilitySystem.AttributeSystem.DOTS.Components;
using MyGameplayAbilitySystem.AttributeSystem.DOTS.Components;
using Unity.Entities;
using UnityEngine;

[assembly: RegisterGenericJobType(typeof(GameplayAbilitySystem.AttributeSystem.DOTS.Components.AttributeSystem.AttributeSystemJob<AttributeHealth.Current, AttributeHealth.Base, AttributeHealth.Modifiers>))]
[assembly: RegisterGenericJobType(typeof(GameplayAbilitySystem.AttributeSystem.DOTS.Components.AttributeSystem.AttributeSystemJob<AttributeMaxHealth.Current, AttributeMaxHealth.Base, AttributeMaxHealth.Modifiers>))]


namespace MyGameplayAbilitySystem.AttributeSystem.DOTS.Components
{

    public class MyBaseAttributesSystem : GameplayAbilitySystem.AttributeSystem.DOTS.Components.AttributeSystem
    {
        public void CreateTestEntities()
        {
            var archetype = EntityManager.CreateArchetype(typeof(AttributeHealth.Current), typeof(AttributeHealth.Base), typeof(AttributeHealth.Modifiers));
            for (var i = 0; i < 10000; i++)
            {
                var entity = EntityManager.CreateEntity(archetype);
                EntityManager.SetComponentData<AttributeHealth.Base>(entity, new AttributeHealth.Base() { Value = Random.Range(0, 100) });
                EntityManager.SetComponentData<AttributeHealth.Modifiers>(entity, new AttributeHealth.Modifiers() { Add = Random.Range(0, 100), Multiply = Random.Range(0.8f, 1.2f) });
            }
        }
        protected override void S()
        {
            attributes.Add(new AttributeHealth());
            attributes.Add(new AttributeMaxHealth());
            CreateTestEntities();
        }
    }
}