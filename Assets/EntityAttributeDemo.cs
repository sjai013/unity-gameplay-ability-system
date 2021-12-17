using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.AttributeSystem.DOTS.Components;
using MyGameplayAbilitySystem.AttributeSystem.DOTS.Components;
using Unity.Entities;
using UnityEngine;

public class EntityAttributeDemo : MonoBehaviour
{
    // Start is called before the first frame update
    CreateTestAttributeSystem system;
    public int entityIndex;
    public PlayerAttribute Health;
    public PlayerAttribute MaxHealth;
    public PlayerAttribute Mana;
    public PlayerAttribute MaxMana;
    public PlayerAttribute Speed;

    // Update is called once per frame
    void Update()
    {
        var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CreateTestAttributeSystem>();
        if (entityIndex > system.AttributeEntities.Count - 1 || entityIndex < 0)
        {
            return;
        }
        Health = system.EntityManager.GetComponentData<AttributeHealth>(system.AttributeEntities[entityIndex]).Value;
        MaxHealth = system.EntityManager.GetComponentData<AttributeMaxHealth>(system.AttributeEntities[entityIndex]).Value;
        Mana = system.EntityManager.GetComponentData<AttributeMana>(system.AttributeEntities[entityIndex]).Value;
        MaxMana = system.EntityManager.GetComponentData<AttributeMaxMana>(system.AttributeEntities[entityIndex]).Value;
        Speed = system.EntityManager.GetComponentData<AttributeSpeed>(system.AttributeEntities[entityIndex]).Value;
    }
}
