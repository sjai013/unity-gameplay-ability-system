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

using System;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Systems {

    public abstract class GenericAbilitySystem<T> : AbilitySystem<T>
    where T : struct, IAbilityTagComponent, IComponentData {

        protected EntityQuery CooldownEffectsQuery;
        protected EntityQuery GrantedAbilityQuery;
        protected abstract ComponentType[] CooldownEffects { get; }
        protected override void OnCreate() {
            InitialiseQueries();
        }

        protected void InitialiseQueries() {

            EntityQueryDesc _cooldownQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>() },
                Any = CooldownEffects
            };

            CooldownEffectsQuery = GetEntityQuery(_cooldownQueryDesc);
            GrantedAbilityQuery = GetEntityQuery(ComponentType.ReadOnly<AbilitySystemActor>(), ComponentType.ReadWrite<T>());
        }

        protected override JobHandle CheckAbilityAvailable(JobHandle inputDeps) {
            // Check the granted ability entity for this ability.  Usually, if cooldown <= 0, ability is not available.

            // Any other logic that determines whether the 
            return inputDeps;
        }

        protected override JobHandle CooldownJobs(JobHandle inputDeps) {
            NativeMultiHashMap<Entity, GameplayEffectDurationComponent> Cooldowns = new NativeMultiHashMap<Entity, GameplayEffectDurationComponent>(CooldownEffectsQuery.CalculateEntityCount() * 2 + GrantedAbilityQuery.CalculateEntityCount(), Allocator.TempJob);

            // Collect all effects which act as cooldowns for this ability
            inputDeps = new GatherCooldownGameplayEffectsJob
            {
                GameplayEffectDurations = Cooldowns.AsParallelWriter()
            }.Schedule(CooldownEffectsQuery, inputDeps);

            // Add a default value of '0' for all entities as well
            inputDeps = new CooldownAbilityIsZeroIfAbsentJob
            {
                GameplayEffectDurations = Cooldowns.AsParallelWriter()
            }.Schedule(GrantedAbilityQuery, inputDeps);

            // Get the effect with the longest cooldown remaining
            inputDeps = new GatherLongestCooldownPerEntity
            {
                GameplayEffectDurationComponent = Cooldowns
            }.Schedule(GrantedAbilityQuery, inputDeps);

            Cooldowns.Dispose(inputDeps);
            return inputDeps;
        }

    }
}