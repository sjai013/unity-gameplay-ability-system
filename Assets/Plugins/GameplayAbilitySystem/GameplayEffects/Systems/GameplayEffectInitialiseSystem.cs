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
    public struct GameplayEffectActivatedSystemStateComponent : ISystemStateComponentData { }

    [UpdateInGroup(typeof(GameplayEffectGroupUpdateBeginSystem))]
    public class GameplayEffectInitialiseSystem : JobComponentSystem {
        private BeginInitializationEntityCommandBufferSystem m_EntityCommandBuffer;
        private EntityQuery m_AddSystemState;
        private EntityQuery m_RemoveSystemState;

        public EntityQuery GameplayEffectsPendingRemovalQuery => m_RemoveSystemState;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
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
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
                var chunkEntities = chunk.GetNativeArray(EntityType);
                for (var i = 0; i < chunk.Count; i++) {
                    var Entity = chunkEntities[i];
                    Ecb.AddComponent<GameplayEffectActivatedSystemStateComponent>(i, Entity);
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

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = new AddSystemStateJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                EntityType = GetArchetypeChunkEntityType(),
            }.Schedule(m_AddSystemState, inputDeps);

            inputDeps = new CleanupEntityJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                EntityType = GetArchetypeChunkEntityType(),
            }.Schedule(m_RemoveSystemState, inputDeps);
            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }

}