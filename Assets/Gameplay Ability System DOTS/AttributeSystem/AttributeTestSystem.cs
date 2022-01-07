using System.Collections.Generic;
using MyGameplayAbilitySystem.Attributes;
using Unity.Collections;
using Unity.Entities;

public class AttributeTestSystem : SystemBase
{
    const int size = 100000;
    protected override void OnCreate()
    {
        Test1();
    }

    private void Test1()
    {
        NativeArray<Entity> entities1 = new NativeArray<Entity>(size, Allocator.Temp);
        EntityManager.CreateEntity(PrimaryAttributeArchetypeFactory.PrimaryAttributeArchetype(EntityManager), entities1);
        entities1.Dispose();

        NativeArray<Entity> entities2 = new NativeArray<Entity>(size, Allocator.Temp);
        EntityManager.CreateEntity(PrimaryAttributeArchetypeFactory.HeroAttributeArchetype(EntityManager), entities2);
        entities2.Dispose();
    }


    protected override void OnUpdate()
    {

    }
}


public static class ArchetypeExtensions
{
    public static EntityArchetype CreateArchetype(this EntityManager em, params ComponentType[][] componentTypes)
    {
        List<ComponentType> componentTypesList = new List<ComponentType>();
        for (var i = 0; i < componentTypes.Length; i++)
        {
            componentTypesList.AddRange(componentTypes[i]);
        }
        return em.CreateArchetype(componentTypesList.ToArray());
    }
}