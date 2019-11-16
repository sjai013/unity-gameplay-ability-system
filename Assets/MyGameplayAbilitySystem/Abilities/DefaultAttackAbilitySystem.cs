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
using GameplayAbilitySystem.Abilities.Systems;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.ExtensionMethods;
using MyGameplayAbilitySystem.AbilitySystem.Enums;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MyGameplayAbilitySystem.Abilities {
    public struct DefaultAttackAbilityTag : IAbilityTagComponent, IComponentData { }



    public class DefaultAttackAbilityCooldownSystem : GenericAbilityCooldownSystem<DefaultAttackAbilityTag> {
        protected override ComponentType[] CooldownEffects => new ComponentType[] { ComponentType.ReadOnly<GlobalCooldownGameplayEffectComponent>() };

    }

    public class DefaultAttackAbilityStateUpdateSystem : GenericAbilityStateUpdateSystem<DefaultAttackAbilityTag> {

        [RequireComponentTag(typeof(DefaultAttackAbilityTag))]
        public struct Job : IJobForEach<AbilityCooldownComponent, AbilityStateComponent> {
            public void Execute([ReadOnly] ref AbilityCooldownComponent cooldown, ref AbilityStateComponent state) {

                if (cooldown.Value.RemainingTime <= 0) {
                    state = (int)AbilityStates.READY;
                } else {
                    state = (int)AbilityStates.ON_COOLDOWN;
                }
            }
        }
        protected override JobHandle StateJobs(JobHandle inputDeps) {
            inputDeps = inputDeps.ScheduleJob(new Job(), abilityQuery);
            return inputDeps;
        }
    }

}
