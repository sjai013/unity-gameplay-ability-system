/*
 * Created on Wed Dec 11 2019
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


using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.GameplayEffects.Systems {
    public struct GameplayEffectActivatedSystemStateComponent : ISystemStateComponentData {
        public Entity TargetEntity;
    }

    [UpdateInGroup(typeof(GameplayEffectGroupUpdateBeginSystem))]
    public class GameplayEffectInitialiseSystem : JobComponentSystem {
        private BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        private EntityQuery m_AddSystemState;
        private EntityQuery m_RemoveSystemState;

        public EntityQuery GameplayEffectsPendingRemovalQuery => m_RemoveSystemState;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_AddSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectTargetComponent>() },
                None = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectActivatedSystemStateComponent>() }
            });

            m_RemoveSystemState = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectActivatedSystemStateComponent>() },
                None = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectTargetComponent>() }
            });
        }

        /// <summary>
        /// Add system state to indicate the gameplay effect is active
        /// </summary>
        [BurstCompile]
        struct AddSystemStateJob : IJobChunk {
            public EntityCommandBuffer.Concurrent Ecb;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<GameplayEffectTargetComponent> Targets;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
                var chunkEntities = chunk.GetNativeArray(EntityType);
                var targetComponentChunk = chunk.GetNativeArray(Targets);
                for (var i = 0; i < chunk.Count; i++) {
                    var Entity = chunkEntities[i];
                    var targetComponent = targetComponentChunk[i];
                    Ecb.AddComponent<GameplayEffectActivatedSystemStateComponent>(i, Entity, new GameplayEffectActivatedSystemStateComponent { TargetEntity = targetComponent });
                }
            }
        }


        /// <summary>
        /// Remove the system state from gameplay effects, so we can handle cleanup as required
        /// </summary>

        [BurstCompile]
        struct CleanupEntityJob : IJobChunk {
            public EntityCommandBuffer.Concurrent Ecb;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
                var chunkEntities = chunk.GetNativeArray(EntityType);
                for (var i = 0; i < chunk.Count; i++) {
                    var Entity = chunkEntities[i];
                    Ecb.RemoveComponent<GameplayEffectActivatedSystemStateComponent>(i, Entity);
                }
            }
        }

        /// <summary>
        /// Appends this GameplayEffect to the actor's dynamic buffer
        /// </summary>
        [BurstCompile]
        struct AddElementToDynamicBuffer : IJob {
            public BufferFromEntity<GameplayEffectBufferElement> bufferFromEntity;

            [DeallocateOnJobCompletion]
            public NativeArray<GameplayEffectTargetComponent> targetComponents;

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
            public BufferFromEntity<GameplayEffectBufferElement> bufferFromEntity;

            [DeallocateOnJobCompletion]
            public NativeArray<GameplayEffectActivatedSystemStateComponent> targetComponents;

            [DeallocateOnJobCompletion]
            public NativeArray<Entity> entities;
            public void Execute() {
                for (var i = 0; i < entities.Length; i++) {
                    // Get the dynamic buffer on the actor
                    var buffer = bufferFromEntity[targetComponents[i].TargetEntity];

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
                Targets = GetArchetypeChunkComponentType<GameplayEffectTargetComponent>()
            }.Schedule(m_AddSystemState, inputDeps);

            var inputDeps2 = new CleanupEntityJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                EntityType = GetArchetypeChunkEntityType(),
            }.Schedule(m_RemoveSystemState, inputDeps);

            inputDeps = JobHandle.CombineDependencies(inputDeps, inputDeps1, inputDeps2);

            inputDeps = new RemoveElementFromDynamicBuffer
            {
                bufferFromEntity = GetBufferFromEntity<GameplayEffectBufferElement>(),
                entities = m_RemoveSystemState.ToEntityArray(Allocator.TempJob),
                targetComponents = m_RemoveSystemState.ToComponentDataArray<GameplayEffectActivatedSystemStateComponent>(Allocator.TempJob)

            }.Schedule(inputDeps);

            inputDeps = new AddElementToDynamicBuffer
            {
                bufferFromEntity = GetBufferFromEntity<GameplayEffectBufferElement>(),
                entities = m_AddSystemState.ToEntityArray(Allocator.TempJob),
                targetComponents = m_AddSystemState.ToComponentDataArray<GameplayEffectTargetComponent>(Allocator.TempJob)
            }.Schedule(inputDeps);


            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }

}