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
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Common.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Abilities.Systems.Generic {
    [UpdateInGroup(typeof(AbilityUpdateEndSystemGroup))]
    public abstract class GenericAssignAbilityIdentifierSystem<T> : JobComponentSystem
    where T : struct, IComponentData {

        BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        protected abstract int AbilityIdentifier { get; }
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        [ExcludeComponent(typeof(AbilityIdentifierComponent))]
        [RequireComponentTag(typeof(AbilitySystemActorTransformComponent), typeof(AbilityOwnerComponent), typeof(AbilityCooldownComponent))]
        struct Job : IJobForEachWithEntity<AbilityStateComponent, T> {
            public EntityCommandBuffer.Concurrent Ecb;
            public int AbilityIdentifier;
            public void Execute(Entity entity, int index, ref AbilityStateComponent c0, [ReadOnly] ref T c1) {
                Ecb.AddComponent(index, entity, new AbilityIdentifierComponent { Value = AbilityIdentifier });
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = new Job()
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                AbilityIdentifier = AbilityIdentifier
            }.Schedule(this, inputDeps);
            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);

            return inputDeps;
        }
    }
}
