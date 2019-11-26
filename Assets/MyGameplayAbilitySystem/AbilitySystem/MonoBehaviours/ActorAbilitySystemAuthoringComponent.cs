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
using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.ScriptableObjects;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using MyGameplayAbilitySystem.Abilities;
using Unity.Entities;
using UnityEngine;

namespace MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    [RequireComponent(typeof(ActorAbilitySystem))]

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
        [SerializeReference]
        private CharacterAttributesScriptableObject Attributes;

        [SerializeReference]
        private GrantedAbilitiesScriptableObject GrantedAbilities;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var abilityOwnerEntity = CreateAttributeEntities(entity, dstManager);
            var abilitySystemGrantedAbilityEntity = CreateGrantedAbilityEntities(entity, dstManager, abilityOwnerEntity);

            var actorAbilitySystem = transform.GetComponent<ActorAbilitySystem>();
            actorAbilitySystem.AbilityOwnerEntity = abilityOwnerEntity;
            actorAbilitySystem.AbilitySystemActorTransformEntity = entity;
            // Create some dummy health attributes to simulate a scenario where there are many modifiers for an attribute active in the world
            CreateEntities<GameplayAbilitySystem.Attributes.Components.Operators.Add, HealthAttributeComponent>.CreateAttributeOperEntities(dstManager, abilityOwnerEntity);
            CreateEntities<GameplayAbilitySystem.Attributes.Components.Operators.Multiply, HealthAttributeComponent>.CreateAttributeOperEntities(dstManager, abilityOwnerEntity);
            CreateEntities<GameplayAbilitySystem.Attributes.Components.Operators.Divide, HealthAttributeComponent>.CreateAttributeOperEntities(dstManager, abilityOwnerEntity);

            // Create some dummy cooldown gameplay effects to simulate a scenario where an ability is on cooldown due to a gameplay effect
            TestAbilitySystemCooldown<Fire1AbilityTag>(dstManager, abilityOwnerEntity);
            TestAbilityCost<Fire1AbilityTag>(dstManager, abilityOwnerEntity);

            TestAbilitySystemCooldown<DefaultAttackAbilityTag>(dstManager, abilityOwnerEntity);
            TestAbilityCost<DefaultAttackAbilityTag>(dstManager, abilityOwnerEntity);
        }

        private List<Entity> CreateGrantedAbilityEntities(Entity entity, EntityManager dstManager, Entity abilitySystemAttributesEntity) {

            var grantedAbilities = new List<ComponentType>();
            if (GrantedAbilities != null && GrantedAbilities.Components != null) {
                grantedAbilities = GrantedAbilities.ComponentTypes;
            }

            var entities = new List<Entity>();

            for (var i = 0; i < grantedAbilities.Count; i++) {
                var abilityType = grantedAbilities[i];
                var grantedAbilityArchetype = dstManager.CreateArchetype(typeof(AbilitySystemActorTransformComponent), abilityType, typeof(AbilityOwnerComponent), typeof(AbilityCooldownComponent), typeof(AbilityStateComponent));
                var abilitySystemGrantedAbilityEntity = dstManager.CreateEntity(grantedAbilityArchetype);

                dstManager.SetComponentData(abilitySystemGrantedAbilityEntity, new AbilitySystemActorTransformComponent
                {
                    Value = entity
                });
                dstManager.SetComponentData(abilitySystemGrantedAbilityEntity, new AbilityOwnerComponent
                {
                    Value = abilitySystemAttributesEntity
                });
                // dstManager.SetComponentData(abilitySystemGrantedAbilityEntity, new AbilityIdentifierComponent
                // {
                //     Value = 1
                // });
                dstManager.SetName(abilitySystemGrantedAbilityEntity, this.gameObject.name + " - Granted Ability - " + abilityType.GetManagedType().Name);
                entities.Add(abilitySystemGrantedAbilityEntity);
            }

            return entities;
        }

        private Entity CreateAttributeEntities(Entity entity, EntityManager entityManager) {
            // Get reference to character attribute component on script, and list of attributes
            var attributeTypes = new List<ComponentType>();
            if (Attributes != null && Attributes.Components != null) {
                attributeTypes = Attributes.ComponentTypes;
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
        private void TestAbilitySystemCooldown<T>(EntityManager dstManager, Entity abilitySystemEntity)
        where T : struct, IAbilityTagComponent {
            (new T()).CreateCooldownEntities(dstManager, abilitySystemEntity);
        }

        private void TestAbilityCost<T>(EntityManager dstManager, Entity abilitySystemEntity)
     where T : struct, IAbilityTagComponent {
            (new T()).CreateSourceAttributeModifiers(dstManager, abilitySystemEntity);
        }
    }


    internal class CreateEntities<TOper, TAttribute>
    where TOper : struct, IAttributeOperator, IComponentData
    where TAttribute : struct, IAttributeComponent, IComponentData {
        public static void CreateAttributeOperEntities(EntityManager EntityManager, Entity ActorEntity) {

            var archetype = EntityManager.CreateArchetype(
                typeof(TOper),
                typeof(AttributeComponentTag<TAttribute>),
                typeof(AttributeModifier<TOper, TAttribute>),
                typeof(AttributesOwnerComponent)
            );

            var random = new Unity.Mathematics.Random((uint)ActorEntity.Index);


            for (var i = 0; i < 5; i++) {
                var entity = EntityManager.CreateEntity(archetype);
                EntityManager.SetComponentData(entity, new GameplayAbilitySystem.Attributes.Components.AttributeModifier<TOper, TAttribute>()
                {
                    Value = random.NextFloat(0, 50)
                });

                EntityManager.SetComponentData(entity, new AttributesOwnerComponent()
                {
                    Value = ActorEntity
                });
            }
        }
    }

    internal class CreatePlayer {
        public static Entity CreatePlayerEntity(EntityManager EntityManager) {
            var playerArchetype = EntityManager.CreateArchetype(
                typeof(HealthAttributeComponent),
                typeof(AbilitySystemActorTransformComponent)
            );

            return EntityManager.CreateEntity(playerArchetype);
        }
    }

}