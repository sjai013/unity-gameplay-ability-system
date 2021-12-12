using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public struct SharedComponentData : ISharedComponentData
{
    public Entity ent;
}

public struct ComponentData : IComponentData
{
    public int A;
}


public class DynamicBufferTest : SystemBase
{
    protected override void OnCreate()
    {
        var archetype = EntityManager.CreateArchetype(
            World.GetOrCreateSystem<HealthAttributeSystem>().GetArchetype().Concat(
            World.GetOrCreateSystem<MaxHealthAttributeSystem>().GetArchetype()).ToArray()
        );


        Entity[] entities = new Entity[1000];
        for (var i = 0; i < 1000; i++)
        {
            var entity = EntityManager.CreateEntity(archetype);
            EntityManager.SetComponentData<HealthAttributeSystem.BaseValue>(entity, new HealthAttributeSystem.BaseValue() { Value = 100 });
            EntityManager.SetComponentData<MaxHealthAttributeSystem.BaseValue>(entity, new MaxHealthAttributeSystem.BaseValue() { Value = 100 });


            var buffer_health = EntityManager.GetBuffer<HealthAttributeSystem.AttributeModifierBufferElement>(entity);
            for (var j = 0; j < 1000; j++)
            {
                buffer_health.Add(new HealthAttributeSystem.AttributeModifierBufferElement() { Modifier = EAttributeModifiers.Add, Value = Random.Range(0, 100) });
                buffer_health.Add(new HealthAttributeSystem.AttributeModifierBufferElement() { Modifier = EAttributeModifiers.Multiply, Value = Random.Range(0.5f, 1.5f) });
            }

            var buffer_maxHealth = EntityManager.GetBuffer<MaxHealthAttributeSystem.AttributeModifierBufferElement>(entity);
            for (var j = 0; j < 1000; j++)
            {
                buffer_maxHealth.Add(new MaxHealthAttributeSystem.AttributeModifierBufferElement() { Modifier = EAttributeModifiers.Add, Value = Random.Range(0, 100) });
                buffer_maxHealth.Add(new MaxHealthAttributeSystem.AttributeModifierBufferElement() { Modifier = EAttributeModifiers.Multiply, Value = Random.Range(0.5f, 1.5f) });
            }
            entities[i] = entity;
        }


        // for (var i = 0; i < 1000; i++)
        // {
        //     var entityIndex = Random.Range(0, 99);
        //     EntityManager.SetSharedComponentData(entities[i], new SharedComponentData() { ent = entities[entityIndex] });

        // }


    }

    protected override void OnUpdate()
    {
        var scds = new List<SharedComponentData>();
        EntityManager.GetAllUniqueSharedComponentData<SharedComponentData>(scds);

        for (var i = 0; i < scds.Count; i++)
        {
            Entities.ForEach((ref ComponentData component) =>
            {

            }).Schedule();
        }
    }


}
