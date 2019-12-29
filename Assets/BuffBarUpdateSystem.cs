/*
 * Created on Mon Dec 23 2019
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
using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects.Components;
using GameplayAbilitySystem.GameplayEffects.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysUpdateSystem]
public class BuffBarUpdateSystem : JobComponentSystem {
    EntityQuery m_Query;
    private Dictionary<Entity, List<BuffBarManager>> BuffBars = new Dictionary<Entity, List<BuffBarManager>>();
    public void RegisterBuffBar(BuffBarManager buffBarManager) {
        // Add the manager to container
        if (!BuffBars.TryGetValue(buffBarManager.AbilitySystem.AbilityOwnerEntity, out var buffBarsForPlayer)) {
            buffBarsForPlayer = new List<BuffBarManager>();
            BuffBars.Add(buffBarManager.AbilitySystem.AbilityOwnerEntity, buffBarsForPlayer);
        }
        buffBarsForPlayer.Add(buffBarManager);
    }

    public void UnregisterBuffBar(BuffBarManager buffBarManager) {
        // Remove the manager from container
        if (BuffBars.TryGetValue(buffBarManager.AbilitySystem.AbilityOwnerEntity, out var buffBars)) {
            buffBars.Remove(buffBarManager);
        }
    }

    protected override void OnCreate() {
        m_Query = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>(), ComponentType.ReadOnly<GameplayEffectBuffIndex>() },
            }
        );
    }
    struct EffectBuffTuple : IComparable<EffectBuffTuple> {
        public GameplayEffectDurationComponent DurationComponent;
        public GameplayEffectBuffIndex BuffComponent;
        public int CompareTo(EffectBuffTuple other) {
            return DurationComponent.Value.NominalDuration < other.DurationComponent.Value.NominalDuration ? 1 : -1;
        }
    }


    [BurstCompile]
    struct UpdateJob : IJob {

        [ReadOnly]
        public DynamicBuffer<GameplayEffectBufferElement> GEBuffer;
        [ReadOnly]
        public ComponentDataFromEntity<GameplayEffectDurationComponent> DurationComponents;
        [ReadOnly]
        public ComponentDataFromEntity<GameplayEffectTargetComponent> TargetComponents;
        [ReadOnly]
        public ComponentDataFromEntity<GameplayEffectBuffIndex> BuffIndexComponents;

        public NativeArray<EffectBuffTuple> BuffDurationTuple;
        public Entity ActorEntity;

        public void Execute() {
            for (var i = 0; i < GEBuffer.Length; i++) {
                var GEEntity = GEBuffer[i];
                BuffDurationTuple[i] = new EffectBuffTuple
                {
                    BuffComponent = BuffIndexComponents[GEEntity],
                    DurationComponent = DurationComponents[GEEntity]
                };
            }
            BuffDurationTuple.Sort<EffectBuffTuple>();
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        // Get reference to the dynamic buffer containing the GE entities
        var GEBuffer = GetBufferFromEntity<GameplayEffectBufferElement>(true);
        var DurationComponents = GetComponentDataFromEntity<GameplayEffectDurationComponent>(true);
        var TargetComponents = GetComponentDataFromEntity<GameplayEffectTargetComponent>(true);
        var BuffIndexComponents = GetComponentDataFromEntity<GameplayEffectBuffIndex>(true);
        foreach (var buffBarManager in BuffBars) {
            var buffer = GEBuffer[buffBarManager.Key];
            var buffDurationTuple = new NativeArray<EffectBuffTuple>(buffer.Length, Allocator.TempJob);
            var job = new UpdateJob
            {
                ActorEntity = buffBarManager.Key,
                BuffDurationTuple = buffDurationTuple,
                BuffIndexComponents = BuffIndexComponents,
                DurationComponents = DurationComponents,
                TargetComponents = TargetComponents,
                GEBuffer = buffer
            }.Schedule(inputDeps);
            job.Complete();
            for (var iManager = 0; iManager < buffBarManager.Value.Count; iManager++) {
                // Do safety and sanity checks, to make sure this GameObject is worth updating
                if (buffBarManager.Value == null) continue;
                if (!buffBarManager.Value[iManager].enabled) continue;

                var managerGameObject = buffBarManager.Value[iManager];
                for (var i = 0; i < buffDurationTuple.Length; i++) {
                    managerGameObject.BuffUIObject[i].CooldownOverlay.fillAmount = buffDurationTuple[i].DurationComponent.PercentRemaining;
                    if (managerGameObject.BuffIconForIdentifier.TryGetValue(buffDurationTuple[i].BuffComponent.Value, out var buffIcon)) {
                        managerGameObject.BuffUIObject[i].ImageIcon.sprite = buffIcon.Sprite;
                        managerGameObject.BuffUIObject[i].ImageIcon.color = buffIcon.SpriteColor;
                    } else {
                        managerGameObject.BuffUIObject[i].ImageIcon.sprite = null;
                        managerGameObject.BuffUIObject[i].ImageIcon.color = Color.clear;
                    }
                }
                // Reset all other images to 0
                for (var i = buffDurationTuple.Length; i < buffBarManager.Value[iManager].BuffUIObject.Count; i++) {
                    managerGameObject.BuffUIObject[i].CooldownOverlay.fillAmount = 0;
                    managerGameObject.BuffUIObject[i].ImageIcon.sprite = null;
                    managerGameObject.BuffUIObject[i].ImageIcon.color = Color.clear;
                }
            }
            buffDurationTuple.Dispose();
        }
        return inputDeps;
    }

}


