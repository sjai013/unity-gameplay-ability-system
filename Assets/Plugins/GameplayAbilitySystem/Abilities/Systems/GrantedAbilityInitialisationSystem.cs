/*
 * Created on Wed Jan 01 2020
 *
 * The MIT License (MIT)
 * Copyright (c) 2020 Sahil Jain
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


using GameplayAbilitySystem.AbilitySystem.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Abilities.Systems {

    public struct GrantedAbilityBufferElement : IBufferElementData {
        public Entity Value;
        public static implicit operator Entity(GrantedAbilityBufferElement e) { return e.Value; }
        public static implicit operator GrantedAbilityBufferElement(Entity e) { return new GrantedAbilityBufferElement { Value = e }; }
    }

    public class GrantedAbilityInitialisationSystem : JobComponentSystem {
        public struct GrantedAbilitySystemStateComponent : ISystemStateComponentData {
            public Entity Owner;
        }

        private BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        private EntityQuery m_AddSystemState;
        private EntityQuery m_RemoveSystemState;

        public EntityQuery GameplayEffectsPendingRemovalQuery => m_RemoveSystemState;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_AddSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<AbilityOwnerComponent>() },
                None = new ComponentType[] { ComponentType.ReadOnly<GrantedAbilitySystemStateComponent>() }
            });

            m_RemoveSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GrantedAbilitySystemStateComponent>() },
                None = new ComponentType[] { ComponentType.ReadOnly<AbilityOwnerComponent>() }
            });
        }

        [BurstCompile]
        struct AddSystemStateJob : IJobChunk {
            public EntityCommandBuffer.Concurrent Ecb;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<AbilityOwnerComponent> Targets;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
                var chunkEntities = chunk.GetNativeArray(EntityType);
                var targetComponentChunk = chunk.GetNativeArray(Targets);
                for (var i = 0; i < chunk.Count; i++) {
                    var Entity = chunkEntities[i];
                    var targetComponent = targetComponentChunk[i];
                    Ecb.AddComponent<GrantedAbilitySystemStateComponent>(i, Entity, new GrantedAbilitySystemStateComponent { Owner = targetComponent });
                }
            }
        }

        [BurstCompile]
        struct CleanupEntityJob : IJobChunk {
            public EntityCommandBuffer.Concurrent Ecb;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
                var chunkEntities = chunk.GetNativeArray(EntityType);
                for (var i = 0; i < chunk.Count; i++) {
                    var Entity = chunkEntities[i];
                    Ecb.RemoveComponent<GrantedAbilitySystemStateComponent>(i, Entity);
                }
            }
        }

        /// <summary>
        /// Appends this GameplayEffect to the actor's dynamic buffer
        /// </summary>
        [BurstCompile]
        struct AddElementToDynamicBuffer : IJob {
            public BufferFromEntity<GrantedAbilityBufferElement> bufferFromEntity;

            [DeallocateOnJobCompletion]
            public NativeArray<AbilityOwnerComponent> targetComponents;

            [DeallocateOnJobCompletion]
            public NativeArray<Entity> entities;

            public void Execute() {
                for (var i = 0; i < entities.Length; i++) {
                    bufferFromEntity[targetComponents[i]].Add(entities[i]);
                }
            }
        }

        /// <summary>
        /// Removes this GameplayEffect from the actor's dynamic buffer
        /// </summary>
        struct RemoveElementFromDynamicBuffer : IJob {
            public BufferFromEntity<GrantedAbilityBufferElement> bufferFromEntity;

            [DeallocateOnJobCompletion]
            public NativeArray<GrantedAbilitySystemStateComponent> targetComponents;

            [DeallocateOnJobCompletion]
            public NativeArray<Entity> entities;
            public void Execute() {
                for (var i = 0; i < entities.Length; i++) {
                    // Get the dynamic buffer on the actor
                    var buffer = bufferFromEntity[targetComponents[i].Owner];

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
            var inputDeps1 = new AddSystemStateJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                EntityType = GetArchetypeChunkEntityType(),
                Targets = GetArchetypeChunkComponentType<AbilityOwnerComponent>()
            }.Schedule(m_AddSystemState, inputDeps);

            var inputDeps2 = new CleanupEntityJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                EntityType = GetArchetypeChunkEntityType(),
            }.Schedule(m_RemoveSystemState, inputDeps);

            inputDeps = JobHandle.CombineDependencies(inputDeps, inputDeps1, inputDeps2);

            inputDeps = new RemoveElementFromDynamicBuffer
            {
                bufferFromEntity = GetBufferFromEntity<GrantedAbilityBufferElement>(),
                entities = m_RemoveSystemState.ToEntityArray(Allocator.TempJob),
                targetComponents = m_RemoveSystemState.ToComponentDataArray<GrantedAbilitySystemStateComponent>(Allocator.TempJob)

            }.Schedule(inputDeps);

            inputDeps = new AddElementToDynamicBuffer
            {
                bufferFromEntity = GetBufferFromEntity<GrantedAbilityBufferElement>(),
                entities = m_AddSystemState.ToEntityArray(Allocator.TempJob),
                targetComponents = m_AddSystemState.ToComponentDataArray<AbilityOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDeps);


            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }

}