/*
 * Created on Mon Nov 04 2019
 *
 * The MIT License (MIT)
 * Copyright (c) 2019 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ActorAbilitySystemAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {
    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //    public float scale;
    public CharacterAttributesScriptableObject Attributes;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var abilitySystemAttributesEntity = CreateAttributeEntities(entity, dstManager);
        var abilitySystemGrantedAbilityEntity = CreateGrantedAbilityEntities<DefaultAttackAbilityTag>(entity, dstManager, abilitySystemAttributesEntity);

        TestAbilitySystemCooldown(dstManager, abilitySystemAttributesEntity);
    }

    private Entity CreateGrantedAbilityEntities<T>(Entity entity, EntityManager dstManager, Entity abilitySystemAttributesEntity) {
        var grantedAbilityArchetype = dstManager.CreateArchetype(typeof(AbilitySystemActorTransformComponent), typeof(T), typeof(AbilityOwnerComponent));
        var abilitySystemGrantedAbilityEntity = dstManager.CreateEntity(grantedAbilityArchetype);
        dstManager.SetComponentData(abilitySystemGrantedAbilityEntity, new AbilitySystemActorTransformComponent
        {
            Value = entity
        });
        dstManager.SetComponentData(abilitySystemGrantedAbilityEntity, new AbilityOwnerComponent
        {
            Value = abilitySystemAttributesEntity
        });
        dstManager.SetName(abilitySystemGrantedAbilityEntity, this.gameObject.name + " - Granted Ability - " + typeof(T).Name);
        return abilitySystemGrantedAbilityEntity;
    }

    private Entity CreateAttributeEntities(Entity entity, EntityManager entityManager) {
        List<ComponentType> attributeTypes = new List<ComponentType>();

        // Get reference to character attribute component on script, and list of attributes
        attributeTypes = new List<ComponentType>();
        if (Attributes != null && Attributes.Attributes != null) {
            attributeTypes = Attributes.ComponentArchetype;
        }

        // Add tag component to indicate that this entity represents an actor with attributes
        attributeTypes.Add(typeof(AbilitySystemActorTransformComponent));
        var attributeArchetype = entityManager.CreateArchetype(attributeTypes.ToArray());
        // Create a new entity for this actor
        var abilitySystemAttributesEntity = entityManager.CreateEntity(attributeArchetype);
        entityManager.SetComponentData(abilitySystemAttributesEntity, new AbilitySystemActorTransformComponent
        {
            Value = entity
        });
        entityManager.SetName(abilitySystemAttributesEntity, this.gameObject.name + " - GameplayAbilitySystem");


        // attributeTypes.Add(typeof(DefaultAttackAbilityTag));

        return abilitySystemAttributesEntity;
    }

    /// <summary>
    /// For testing cooldowns
    /// </summary>
    /// <param name="dstManager"></param>
    /// <param name="abilitySystemEntity"></param>
    private void TestAbilitySystemCooldown(EntityManager dstManager, Entity abilitySystemEntity) {

        var cooldownArchetype = dstManager.CreateArchetype(
            typeof(GameplayEffectDurationComponent),
            typeof(GameplayEffectTargetComponent),
            typeof(GlobalCooldownGameplayEffectComponent));

        var cooldownEntity = dstManager.CreateEntity(cooldownArchetype);
        dstManager.SetComponentData<GameplayEffectTargetComponent>(cooldownEntity, abilitySystemEntity);
        dstManager.SetComponentData<GameplayEffectDurationComponent>(cooldownEntity, GameplayEffectDurationComponent.Initialise(10, 1));

        var cooldownEntity2 = dstManager.CreateEntity(cooldownArchetype);
        dstManager.SetComponentData<GameplayEffectTargetComponent>(cooldownEntity2, abilitySystemEntity);
        dstManager.SetComponentData<GameplayEffectDurationComponent>(cooldownEntity2, GameplayEffectDurationComponent.Initialise(5, 1));
    }
}

