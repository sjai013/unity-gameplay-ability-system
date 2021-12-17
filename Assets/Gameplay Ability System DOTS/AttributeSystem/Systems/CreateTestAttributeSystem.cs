
using System.Collections.Generic;
using GameplayAbilitySystem.AttributeSystem.DOTS.Components;
using GameplayAbilitySystem.AttributeSystem.DOTS.Systems;
using MyGameplayAbilitySystem.AttributeSystem.DOTS.Components;
using Unity.Entities;
using UnityEngine;

namespace MyGameplayAbilitySystem.AttributeSystem.DOTS.Components
{

    public class CreateTestAttributeSystem : SystemBase
    {

        public List<Entity> AttributeEntities = new List<Entity>();
        public void CreateTestEntities()
        {
            var archetype = EntityManager.CreateArchetype(typeof(AttributeHealth), typeof(AttributeMaxHealth), typeof(AttributeMana), typeof(AttributeMaxMana), typeof(AttributeSpeed));
            var componentTypes = DynamicAttributeSystem.GetAttributeTypes();

            for (var i = 0; i < 10000; i++)
            {

                var entity = EntityManager.CreateEntity(archetype);

                EntityManager.SetComponentData<AttributeHealth>(entity, new AttributeHealth()
                {
                    Value = CreateRandom()
                });

                EntityManager.SetComponentData<AttributeMaxHealth>(entity, new AttributeMaxHealth()
                {
                    Value = CreateRandom()
                });

                EntityManager.SetComponentData<AttributeMana>(entity, new AttributeMana()
                {
                    Value = CreateRandom()
                });

                EntityManager.SetComponentData<AttributeMaxMana>(entity, new AttributeMaxMana()
                {
                    Value = CreateRandom()
                });

                EntityManager.SetComponentData<AttributeSpeed>(entity, new AttributeSpeed()
                {
                    Value = CreateRandom()
                });


                AttributeEntities.Add(entity);
            }
        }

        protected override void OnCreate()
        {
            CreateTestEntities();
        }

        protected override void OnUpdate()
        {
            //throw new System.NotImplementedException();() 
        }
        public PlayerAttribute CreateRandom()
        {
            var Value = PlayerAttribute.Initialise(Random.Range(0, 100));
            Value.Modifiers.Multiply = Random.Range(-0.2f, 0.2f);
            Value.Modifiers.Add = Random.Range(-50f, 50f);
            return Value;
        }

    }
}