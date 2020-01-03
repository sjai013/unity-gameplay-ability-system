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

public class BuffBarUpdateSystem2 : ComponentSystem {

    struct EffectBuffTuple : IComparable<EffectBuffTuple> {
        public GameplayEffectDurationComponent DurationComponent;
        public GameplayEffectBuffIndex BuffComponent;
        public int CompareTo(EffectBuffTuple other) {
            return DurationComponent.Value.NominalDuration < other.DurationComponent.Value.NominalDuration ? 1 : -1;
        }
    }

    protected override void OnUpdate() {
        var GEBufferFromEntity = GetBufferFromEntity<GameplayEffectBufferElement>(true);
        var DurationComponents = GetComponentDataFromEntity<GameplayEffectDurationComponent>(true);
        var BuffIndexComponents = GetComponentDataFromEntity<GameplayEffectBuffIndex>(true);

        Entities.ForEach<BuffBarManager>((BuffBarManager buffBarManager) => {
            // Get a reference to the DynamicBuffer
            var GEBuffer = GEBufferFromEntity[buffBarManager.AbilitySystem.AbilityOwnerEntity];

            // Create an array to hold the durations
            var BuffDurationTuple = new NativeArray<EffectBuffTuple>(GEBuffer.Length, Allocator.Temp);
            for (var GEBufferIndex = 0; GEBufferIndex < GEBuffer.Length; GEBufferIndex++) {
                var GEEntity = GEBuffer[GEBufferIndex];
                var durationComponent = DurationComponents[GEEntity];
                var buffIndexComponent = BuffIndexComponents[GEEntity];
                BuffDurationTuple[GEBufferIndex] = new EffectBuffTuple
                {
                    BuffComponent = BuffIndexComponents[GEEntity],
                    DurationComponent = DurationComponents[GEEntity]
                };
            }

            //Sort, so we can display in a specified order
            BuffDurationTuple.Sort<EffectBuffTuple>();

            // Update the buff bar icons
            // Do safety and sanity checks, to make sure this GameObject is worth updating
            for (var i = 0; i < BuffDurationTuple.Length; i++) {
                buffBarManager.BuffUIObject[i].CooldownOverlay.fillAmount = BuffDurationTuple[i].DurationComponent.PercentRemaining;
                if (buffBarManager.BuffIconForIdentifier.TryGetValue(BuffDurationTuple[i].BuffComponent.Value, out var buffIcon)) {
                    buffBarManager.BuffUIObject[i].ImageIcon.sprite = buffIcon.Sprite;
                    buffBarManager.BuffUIObject[i].ImageIcon.color = buffIcon.SpriteColor;
                } else {
                    buffBarManager.BuffUIObject[i].ImageIcon.sprite = null;
                    buffBarManager.BuffUIObject[i].ImageIcon.color = Color.clear;
                }
            }
            // Reset all other images to 0
            for (var i = BuffDurationTuple.Length; i < buffBarManager.BuffUIObject.Count; i++) {
                buffBarManager.BuffUIObject[i].CooldownOverlay.fillAmount = 0;
                buffBarManager.BuffUIObject[i].ImageIcon.sprite = null;
                buffBarManager.BuffUIObject[i].ImageIcon.color = Color.clear;
            }
        });
    }
}
