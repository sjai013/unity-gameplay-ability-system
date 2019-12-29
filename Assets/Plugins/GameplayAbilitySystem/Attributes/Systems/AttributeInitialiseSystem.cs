/*
 * Created on Sun Dec 29 2019
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

using GameplayAbilitySystem.Attributes.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {
    public struct AttributeModificationActivatedSystemStateComponent<T> : ISystemStateComponentData {
        public Entity TargetEntity;
    }

    public class TemporaryttributeInitialiseSystem : AttributeInitialisationSystem<TemporaryAttributeModifierTag> { }
    public class PermanentAttributeInitialiseSystem : AttributeInitialisationSystem<PermanentAttributeModifierTag> { }


    [UpdateInGroup(typeof(AttributeSystemGroup))]
    public abstract class AttributeInitialisationSystem<T> : JobComponentSystem
    where T : struct, IComponentData, IAttributeModifierTag {
        private BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        private EntityQuery m_AddSystemState;
        private EntityQuery m_RemoveSystemState;

        public EntityQuery GameplayEffectsPendingRemovalQuery => m_RemoveSystemState;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_AddSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<AttributesOwnerComponent>(), ComponentType.ReadOnly<T>() },
                None = new ComponentType[] { ComponentType.ReadOnly<AttributeModificationActivatedSystemStateComponent<T>>() }
            });

            m_RemoveSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<AttributeModificationActivatedSystemStateComponent<T>>() },
                None = new ComponentType[] { ComponentType.ReadOnly<AttributesOwnerComponent>() }
            });
        }

        struct AddSystemStateComponent : IJobForEachWithEntity<AttributesOwnerComponent, T> {
            public EntityCommandBuffer.Concurrent Ecb;

            public void Execute(Entity entity, int index, [ReadOnly] ref AttributesOwnerComponent owner, [ReadOnly] ref T attributeModifierType) {
                Ecb.AddComponent(index, entity, new AttributeModificationActivatedSystemStateComponent<T> { TargetEntity = owner });
            }
        }

        struct RemoveSystemStateComponent : IJobForEachWithEntity<AttributeModificationActivatedSystemStateComponent<T>> {
            public EntityCommandBuffer.Concurrent Ecb;
            public void Execute(Entity entity, int index, ref AttributeModificationActivatedSystemStateComponent<T> c0) {
                Ecb.RemoveComponent<AttributeModificationActivatedSystemStateComponent<T>>(index, entity);

            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            // Entity has just been created.  Add the system state component
            inputDeps = new AddSystemStateComponent { Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent() }.Schedule(m_AddSystemState, inputDeps);
            inputDeps = new RemoveSystemStateComponent { Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent() }.Schedule(m_RemoveSystemState, inputDeps);

            // Entity has just been destroyed.  Remove the system state component.
            // inputDeps = Entities
            //     .WithNone<AttributesOwnerComponent>()
            //     .WithAll<AttributeModificationActivatedSystemStateComponent>()
            //     .ForEach((Entity attributeEntity, int entityInQueryIndex) => {
            //         Ecb.RemoveComponent<AttributeModificationActivatedSystemStateComponent>(entityInQueryIndex, attributeEntity);
            //     })
            //     .Schedule(inputDeps);

            // Add attribute modification pointer to actor


            // Remove attribute modification pointer from actor


            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}