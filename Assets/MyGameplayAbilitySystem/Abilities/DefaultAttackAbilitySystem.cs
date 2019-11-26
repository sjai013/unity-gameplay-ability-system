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

using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.Abilities.Systems.Generic;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Common.Editor;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;
namespace MyGameplayAbilitySystem.Abilities {

    [AbilitySystemDisplayName("Default Attack Ability")]
    public struct DefaultAttackAbilityTag : IAbilityTagComponent, IComponentData {
        public void CreateCooldownEntities(EntityManager dstManager, Entity actorEntity) {
            var cooldownArchetype1 = dstManager.CreateArchetype(
    typeof(GameplayEffectDurationComponent),
    typeof(GameplayEffectTargetComponent),
    typeof(GlobalCooldownGameplayEffectComponent));

            var cooldownEntity1 = dstManager.CreateEntity(cooldownArchetype1);
            dstManager.SetComponentData<GameplayEffectTargetComponent>(cooldownEntity1, actorEntity);
            dstManager.SetComponentData<GameplayEffectDurationComponent>(cooldownEntity1, GameplayEffectDurationComponent.Initialise(1, UnityEngine.Time.time));
        }

        public void CreateSourceAttributeModifiers(EntityManager dstManager, Entity actorEntity) {
            var archetype = dstManager.CreateArchetype(
                            typeof(GameplayAbilitySystem.Attributes.Components.Operators.Add),
                            typeof(AttributeComponentTag<ManaAttributeComponent>),
                            typeof(AttributeModifier<GameplayAbilitySystem.Attributes.Components.Operators.Add, ManaAttributeComponent>),
                            typeof(AttributesOwnerComponent)
                        );

            var entity = dstManager.CreateEntity(archetype);
            dstManager.SetComponentData(entity, new AttributeModifier<GameplayAbilitySystem.Attributes.Components.Operators.Add, ManaAttributeComponent>()
            {
                Value = -1
            });

            dstManager.SetComponentData(entity, new AttributesOwnerComponent()
            {
                Value = actorEntity
            });
        }
    }

    public class DefaultAttackAbilitySystem {
        public class AbilityCooldownSystem : GenericAbilityCooldownSystem<DefaultAttackAbilityTag> {
            protected override ComponentType[] CooldownEffects =>
                new ComponentType[] {
                    ComponentType.ReadOnly<GlobalCooldownGameplayEffectComponent>()
                };

        }

        public class AssignAbilityIdentifierSystem : GenericAssignAbilityIdentifierSystem<DefaultAttackAbilityTag> {
            protected override int AbilityIdentifier => 1;
        }

    }

}
