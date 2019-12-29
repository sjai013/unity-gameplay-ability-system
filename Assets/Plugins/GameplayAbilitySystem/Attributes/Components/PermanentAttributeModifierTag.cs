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

using GameplayAbilitySystem.Attributes.Systems;
using Unity.Entities;
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<GameplayAbilitySystem.Attributes.Components.PermanentAttributeModifierTag>))]

namespace GameplayAbilitySystem.Attributes.Components {
    public struct PermanentAttributeModifierTag : IComponentData, IAttributeModifierTag {

        public EntityArchetype AttributeModifierArchetype<TAttribute, TOperator>(EntityManager entityManager)
            where TAttribute : struct, IAttributeComponent, IComponentData
            where TOperator : struct, IAttributeOperator {
            var archetype = entityManager.CreateArchetype(
                AttributeOperatorQueryComponents<TAttribute, TOperator>()
            );

            return archetype;
        }

        public ComponentType[] AttributeOperatorQueryComponents<TAttribute, TOperator>()
            where TAttribute : struct, IAttributeComponent, IComponentData
            where TOperator : struct, IAttributeOperator {
            return new ComponentType[] {
                    ComponentType.ReadOnly<AttributeComponentTag<TAttribute>>(),
                    ComponentType.ReadOnly<TOperator>(),
                    ComponentType.ReadOnly<AttributeModifier<TOperator, TAttribute>>(),
                    ComponentType.ReadOnly<AttributesOwnerComponent>(),
                    ComponentType.ReadOnly<PermanentAttributeModifierTag>(),
                };
        }

        public Entity CreateAttributeModifier<TAttribute, TOperator>(EntityManager entityManager, Entity Target, float Value)
            where TAttribute : struct, IAttributeComponent, IComponentData
            where TOperator : struct, IAttributeOperator {
            var archetype = AttributeModifierArchetype<TAttribute, TOperator>(entityManager);
            var entity = entityManager.CreateEntity(archetype);
            entityManager.SetComponentData(entity, new AttributeModifier<TOperator, TAttribute>()
            {
                Value = Value
            });

            entityManager.SetComponentData(entity, new AttributesOwnerComponent()
            {
                Value = Target
            });

            return entity;
        }

        public Entity CreateAttributeModifier<TAttribute, TOperator>(int jobIndex, EntityCommandBuffer.Concurrent Ecb, Entity Target, float Value)
            where TAttribute : struct, IAttributeComponent, IComponentData
            where TOperator : struct, IAttributeOperator {
            var entity = Ecb.CreateEntity(jobIndex);
            var components = AttributeOperatorQueryComponents<TAttribute, TOperator>();
            for (var i = 0; i < components.Length; i++) {
                Ecb.AddComponent(jobIndex, entity, components[i]);
            }

            Ecb.SetComponent(jobIndex, entity, new AttributeModifier<TOperator, TAttribute>()
            {
                Value = Value
            });

            Ecb.SetComponent(jobIndex, entity, new AttributesOwnerComponent()
            {
                Value = Target
            });

            return entity;
        }
    }


}

