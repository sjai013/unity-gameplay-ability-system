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
    public struct AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute> : ISystemStateComponentData {
        public Entity TargetEntity;
    }

    [UpdateInGroup(typeof(AttributeGroupUpdateBeginSystem))]
    public abstract class AttributeInitialisationSystem<TModifierTag, TAttribute> : JobComponentSystem
    where TModifierTag : struct, IComponentData, IAttributeModifierTag
    where TAttribute : struct, IComponentData, IAttributeComponent {
        private BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        private EntityQuery m_AddSystemState;
        private EntityQuery m_RemoveSystemState;

        public EntityQuery GameplayEffectsPendingRemovalQuery => m_RemoveSystemState;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_AddSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<AttributesOwnerComponent>(), ComponentType.ReadOnly<TModifierTag>(), ComponentType.ReadOnly<AttributeComponentTag<TAttribute>>() },
                None = new ComponentType[] { ComponentType.ReadOnly<AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute>>() }
            });

            m_RemoveSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute>>() },
                None = new ComponentType[] { ComponentType.ReadOnly<AttributesOwnerComponent>() }
            });
        }

        struct AddSystemStateComponent : IJobForEachWithEntity<AttributesOwnerComponent, TModifierTag> {
            public EntityCommandBuffer.Concurrent Ecb;

            public void Execute(Entity entity, int index, [ReadOnly] ref AttributesOwnerComponent owner, [ReadOnly] ref TModifierTag attributeModifierType) {
                Ecb.AddComponent(index, entity, new AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute> { TargetEntity = owner });
            }
        }

        struct RemoveSystemStateComponent : IJobForEachWithEntity<AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute>> {
            public EntityCommandBuffer.Concurrent Ecb;
            public void Execute(Entity entity, int index, ref AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute> c0) {
                Ecb.RemoveComponent<AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute>>(index, entity);

            }
        }

        struct AddElementToDynamicBuffer : IJob {
            public BufferFromEntity<AttributeBufferElement<TModifierTag, TAttribute>> bufferFromEntity;

            [DeallocateOnJobCompletion]
            public NativeArray<AttributesOwnerComponent> ownerComponents;

            [DeallocateOnJobCompletion]
            public NativeArray<Entity> entities;
            public void Execute() {
                for (var i = 0; i < entities.Length; i++) {
                    bufferFromEntity[ownerComponents[i]].Add(entities[i]);
                }
            }
        }

        struct RemoveElementFromDynamicBuffer : IJob {
            public BufferFromEntity<AttributeBufferElement<TModifierTag, TAttribute>> bufferFromEntity;

            [DeallocateOnJobCompletion]
            public NativeArray<AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute>> systemStateComponents;

            [DeallocateOnJobCompletion]
            public NativeArray<Entity> entities;
            public void Execute() {
                for (var i = 0; i < entities.Length; i++) {
                    // Get the dynamic buffer on the actor
                    var buffer = bufferFromEntity[systemStateComponents[i].TargetEntity];
                    // Look through the buffer and remove all instances of this effect
                    for (var j = buffer.Length - 1; j >= 0; j--) {
                        if (buffer[j].Value == entities[i]) {
                            buffer.RemoveAt(j);
                        }
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            // Entity has just been created.  Add the system state component
            var inputDeps1 = new AddSystemStateComponent { Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent() }.Schedule(m_AddSystemState, inputDeps);
            var inputDeps2 = new RemoveSystemStateComponent { Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent() }.Schedule(m_RemoveSystemState, inputDeps);
            inputDeps = JobHandle.CombineDependencies(inputDeps, inputDeps1, inputDeps2);

            inputDeps.Complete();
            inputDeps = new RemoveElementFromDynamicBuffer
            {
                bufferFromEntity = GetBufferFromEntity<AttributeBufferElement<TModifierTag, TAttribute>>(),
                entities = m_RemoveSystemState.ToEntityArray(Allocator.TempJob),
                systemStateComponents = m_RemoveSystemState.ToComponentDataArray<AttributeModificationActivatedSystemStateComponent<TModifierTag, TAttribute>>(Allocator.TempJob)

            }.Schedule(inputDeps);

            inputDeps = new AddElementToDynamicBuffer
            {
                bufferFromEntity = GetBufferFromEntity<AttributeBufferElement<TModifierTag, TAttribute>>(),
                entities = m_AddSystemState.ToEntityArray(Allocator.TempJob),
                ownerComponents = m_AddSystemState.ToComponentDataArray<AttributesOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDeps);


            // Add attribute modification pointer to actor


            // Remove attribute modification pointer from actor


            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}