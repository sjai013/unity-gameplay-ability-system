using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

namespace GameplayAbilitySystem.SCD
{

    public class DynamicBufferTest : SystemBase
    {
        protected override void OnCreate()
        {

            var archetype1 = EntityManager.CreateArchetype(
                new ComponentType[] {
                    new ComponentType(typeof(AttributeMasterEntity)),
                    new ComponentType(typeof(HealthAttributeModifier))
                }
            );

            var archetype2 = EntityManager.CreateArchetype(
                new ComponentType[] {
                    new ComponentType(typeof(HealthCurrentValue)),
                    new ComponentType(typeof(HealthBaseValue))
                }
            );

            Entity[] entities_master = new Entity[1000];
            for (var i = 0; i < 1000; i++)
            {
                var entity = EntityManager.CreateEntity(archetype2);
                entities_master[i] = entity;
                EntityManager.SetComponentData(entities_master[i], new HealthBaseValue() { Value = 100f });

            }

            Entity[] entities = new Entity[1000];
            for (var i = 0; i < 1000; i++)
            {
                var entity = EntityManager.CreateEntity(archetype1);
                EntityManager.SetSharedComponentData(entity, new AttributeMasterEntity() { value = entities_master[i] });
                EntityManager.SetComponentData(entity, new HealthAttributeModifier() { Modifier = EAttributeModifiers.Add, Value = 20f });
                entities[i] = entity;
            }


        }

        protected override void OnUpdate()
        {

        }


    }

}
