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


using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using GameplayAbilitySystem.GameplayEffects.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {
    [UpdateInGroup(typeof(AttributeGroupUpdateBeginSystem))]
    public class AttributeCleanupSystem : JobComponentSystem {
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


        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            // For each gameplay effect that is pending removal, cleanup all associated attribute entities
            var entities = gameplayEffectsPendingRemovalQuery.ToEntityArray(Allocator.TempJob, out var jobHandle);
            jobHandle.Complete();
            for (var i = 0; i < entities.Length; i++) {
                var sharedFilterComponent = new ParentGameplayEffectEntity { Value = entities[i] };
                attributesToRemoveQuery.SetFilter<ParentGameplayEffectEntity>(sharedFilterComponent);
                EntityManager.DestroyEntity(attributesToRemoveQuery);
            }

            return inputDeps;

        }
    }
}