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

using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.GameplayEffects.Systems {
    [UpdateInGroup(typeof(GameplayEffectGroupUpdateEndSystem))]
    public class GameplayEffectCleanupSystem : JobComponentSystem {

        private BeginInitializationEntityCommandBufferSystem m_EntityCommandBuffer;
        private EntityQuery m_Group;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            m_Group = GetEntityQuery(ComponentType.ReadOnly<GameplayEffectDurationComponent>());
            // Create test entities
            // var archetype = EntityManager.CreateArchetype(
            //     typeof(GameplayEffectDurationComponent),
            //     typeof(GameplayEffectAttributeEntityComponent)
            // );
        }

        [BurstCompile]
        struct CleanupJob : IJobChunk {
            public EntityCommandBuffer.Concurrent Ecb;
            [ReadOnly] public ArchetypeChunkComponentType<GameplayEffectDurationComponent> DurationComponents;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {

                var chunkDurations = chunk.GetNativeArray(DurationComponents);
                var chunkEntities = chunk.GetNativeArray(EntityType);
                for (var i = 0; i < chunk.Count; i++) {
                    var Entity = chunkEntities[i];
                    var durationComponent = chunkDurations[i];
                    var duration = durationComponent.Value.RemainingTime;
                    if (duration <= 0f) {
                        Ecb.DestroyEntity(chunkIndex, Entity);
                    }
                }
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = new CleanupJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                EntityType = GetArchetypeChunkEntityType(),
                DurationComponents = GetArchetypeChunkComponentType<GameplayEffectDurationComponent>(),
            }.Schedule(m_Group, inputDeps);

            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);

            return inputDeps;
        }
    }
}