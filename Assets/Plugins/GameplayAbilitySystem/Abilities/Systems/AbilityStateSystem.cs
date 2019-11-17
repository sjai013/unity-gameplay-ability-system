/*
 * Created on Mon Nov 17 2019
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
using GameplayAbilitySystem.AbilitySystem.Enums;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Abilities.Systems {
    /// <summary>
    /// Defines the system for update the ability state
    /// </summary>
    /// <typeparam name="T">The Ability</typeparam>
    public abstract class AbilityStateSystem<T> : JobComponentSystem
    where T : struct, IAbilityTagComponent, IComponentData {
        protected abstract JobHandle StateJobs(JobHandle inputDeps);
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = StateJobs(inputDeps);
            return inputDeps;
        }
    }

    [UpdateInGroup(typeof(AbilityGroupUpdateInitialiseSystem))]
    public class ResetAbilityStates : JobComponentSystem {

        struct Job : IJobForEach<AbilityStateComponent> {
            public void Execute(ref AbilityStateComponent state) {
                state = (int)AbilityStates.READY;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = new Job().Schedule(this, inputDeps);
            return inputDeps;
        }
    }
}
