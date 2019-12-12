/*
 * Created on Thu Dec 12 2019
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

using System.Runtime.InteropServices;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects.Systems {
    [UpdateInGroup(typeof(GameplayEffectGroupUpdateEndSystem))]
    public class GameplayEffectTickSystem : JobComponentSystem {
        private BeginInitializationEntityCommandBufferSystem m_EntityCommandBuffer;
        void TickTestFunction(EntityCommandBuffer.Concurrent Ecb) {
            Ecb.CreateEntity(0);
        }

        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var entity = World.Active.EntityManager.CreateEntity(typeof(PeriodicTickComponent));
            World.Active.EntityManager.SetName(entity, "Periodic Tick");


            World.Active.EntityManager.SetComponentData<PeriodicTickComponent>(entity, new PeriodicTickComponent
            {
                TickPeriod = 10,
                TickedDuration = 0,
                Tick = new FunctionPointer<PeriodicTickDelegate>(Marshal.GetFunctionPointerForDelegate((PeriodicTickDelegate)TickTestFunction))

            });
        }

        struct Job : IJobForEach<PeriodicTickComponent> {
            public EntityCommandBuffer.Concurrent Ecb;
            public float DeltaTime;
            public void Execute(ref PeriodicTickComponent tickComponent) {
                tickComponent.TickedDuration -= DeltaTime;
                if (tickComponent.TickedDuration <= 0) {
                    tickComponent.Tick.Invoke(Ecb);
                    // Reset the tick duration, taking into account the overflow amount
                    tickComponent.TickedDuration = tickComponent.TickPeriod + tickComponent.TickedDuration;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = new Job()
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                DeltaTime = Time.deltaTime
            }.Schedule(this, inputDeps);
            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}