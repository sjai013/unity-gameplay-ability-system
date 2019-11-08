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

using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Abilities.Systems {

    /// <summary>
    /// Defines the system for handling ability parameters, such as
    /// current cooldown for each actor.
    /// 
    /// <para>
    /// The set of components that are returned for CooldownQueryDesc must contain <see cref="GameplayEffectTargetComponent"/>
    /// and <see cref="GameplayEffectDurationComponent"/>. Other components should include the relevant gameplay effects.
    /// </para>
    /// 
    /// </summary>
    /// <typeparam name="T">The Ability</typeparam>
    public abstract class AbilityCooldownSystem<T> : JobComponentSystem
    where T : struct, IAbilityTagComponent, IComponentData {
        protected abstract JobHandle CooldownJobs(JobHandle inputDeps);
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = CooldownJobs(inputDeps);
            return inputDeps;
        }

    }
}
