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


using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects.Systems {
    [UpdateInGroup(typeof(GameplayEffectGroupUpdateEndSystem))]
    public class GameplayEffectChildCleanupSystem : JobComponentSystem {
        EntityQuery attributesToRemoveQuery;
        EntityQuery gameplayEffectsPendingRemovalQuery;
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBuffer;

        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            attributesToRemoveQuery = GetEntityQuery(ComponentType.ReadOnly<ParentGameplayEffectEntity>());
            gameplayEffectsPendingRemovalQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectActivatedSystemStateComponent>() },
                None = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectTargetComponent>() }
            });
        }

        struct GatherExpiredEffects : IJobForEachWithEntity<GameplayEffectActivatedSystemStateComponent> {
            public NativeHashMap<Entity, Empty>.ParallelWriter ExpiredGameplayEffects;
            public void Execute(Entity entity, int index, [ReadOnly] ref GameplayEffectActivatedSystemStateComponent _) {
                ExpiredGameplayEffects.TryAdd(entity, new Empty());
            }

        }

        [BurstCompile]
        struct DestroyExpiredEntities : IJobForEachWithEntity<ParentGameplayEffectEntity> {

            [ReadOnly] public NativeHashMap<Entity, Empty> ExpiredGameplayEffects;
            public EntityCommandBuffer.Concurrent Ecb;
            public void Execute(Entity entity, int index, [ReadOnly] ref ParentGameplayEffectEntity parentGameplayEffectEntity) {
                if (!ExpiredGameplayEffects.ContainsKey(parentGameplayEffectEntity.Value)) return;
                Ecb.DestroyEntity(index, entity);
            }
        }

        internal struct Empty { }


        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            NativeHashMap<Entity, Empty> ExpiredGameplayEffects = new NativeHashMap<Entity, Empty>(gameplayEffectsPendingRemovalQuery.CalculateEntityCount(), Allocator.TempJob);
            inputDeps = new GatherExpiredEffects
            {
                ExpiredGameplayEffects = ExpiredGameplayEffects.AsParallelWriter()
            }.Schedule(gameplayEffectsPendingRemovalQuery, inputDeps);
            inputDeps = new DestroyExpiredEntities
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                ExpiredGameplayEffects = ExpiredGameplayEffects
            }.Schedule(attributesToRemoveQuery, inputDeps);
            inputDeps = ExpiredGameplayEffects.Dispose(inputDeps);
            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            // For each gameplay effect that is pending removal, cleanup all associated attribute entities
            // var parentEntities = gameplayEffectsPendingRemovalQuery.CalculateEntityCount();
            // if (parentEntities <= 0) return inputDeps;
            // var entities = gameplayEffectsPendingRemovalQuery.ToEntityArray(Allocator.Temp, out var jobHandle);
            // inputDeps = entities.Dispose(jobHandle);
            // // Copy to a hash map so we can do a quick lookup.  Size will be gameplayEffectsPendingRemoval
            // NativeHashMap<Entity, Empty> ExpiredGameplayEffects = new NativeHashMap<Entity, Empty>(parentEntities, Allocator.TempJob);
            // jobHandle.Complete();
            // var empty = new Entity();
            // for (var i = 0; i < entities.Length; i++) {
            //     ExpiredGameplayEffects.TryAdd(entities[i], empty);
            // }
            // entities.Dispose();

            // inputDeps = new Job
            // {
            //     Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
            //     ExpiredGameplayEffects = ExpiredGameplayEffects
            // }.Schedule(this, inputDeps);
            // inputDeps = ExpiredGameplayEffects.Dispose(inputDeps);
            return inputDeps;

        }
    }
}