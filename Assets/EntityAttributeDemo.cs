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
    public PlayerAttribute Strength;
    public PlayerAttribute Mana;
    public PlayerAttribute MaxMana;
    public PlayerAttribute Speed;
    public PlayerAttribute MaxHealth;

    // Update is called once per frame
    void Update()
    {
        var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CreateTestAttributeSystem>();
        if (entityIndex > system.AttributeEntities.Count - 1 || entityIndex < 0)
        {
            return;
        }

        var entity = system.AttributeEntities[entityIndex];
        Health = system.EntityManager.HasComponent<AttributeHealth>(entity) ? system.EntityManager.GetComponentData<AttributeHealth>(entity).Value : new PlayerAttribute();
        Mana = system.EntityManager.HasComponent<AttributeMana>(entity) ? system.EntityManager.GetComponentData<AttributeMana>(entity).Value : new PlayerAttribute();
        MaxMana = system.EntityManager.HasComponent<AttributeMaxMana>(entity) ? system.EntityManager.GetComponentData<AttributeMaxMana>(entity).Value : new PlayerAttribute();
        Strength = system.EntityManager.HasComponent<AttributeStrength>(entity) ? system.EntityManager.GetComponentData<AttributeStrength>(entity).Value : new PlayerAttribute();
        Speed = system.EntityManager.HasComponent<AttributeSpeed>(entity) ? system.EntityManager.GetComponentData<AttributeSpeed>(entity).Value : new PlayerAttribute();
        MaxHealth = system.EntityManager.HasComponent<AttributeMaxHealth>(entity) ? system.EntityManager.GetComponentData<AttributeMaxHealth>(entity).Value : new PlayerAttribute();
    }
}
